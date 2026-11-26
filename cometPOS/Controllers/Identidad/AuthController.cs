using Microsoft.AspNetCore.Mvc;
using cometPOS.Data;
using cometPOS.DTOs;
using cometPOS.Models;
using cometPOS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;

namespace cometPOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly string[] RolesValidos = { "Admin", "Cajero" };
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly IAuditService _auditService;
    private readonly PasswordHasher<Usuario> _hasher = new();
    public AuthController(AppDbContext context, ITokenService tokenService, IConfiguration config, IAuditService auditService)
    {
        _context = context;
        _tokenService = tokenService;
        _config = config;
        _auditService = auditService;
    }
    
    [Authorize(Roles = "SuperAdmin,Admin")]
    [HttpPost("registro")]
    public async Task<ActionResult> Registrar(RegistrarUsuarioDto dto)
    {
        var esSuperAdmin = User.EsSuperAdmin();
        var empresaId = User.EmpresaId() ?? 0;
        var rolesPermitidos = esSuperAdmin ? RolesValidos : new[] { "Cajero" };

        if (!rolesPermitidos.Contains(dto.Rol))
            return BadRequest($"Rol invalido. Valores permitidos: {string.Join(", ", rolesPermitidos)}.");

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            return Conflict("Ya existe un usuario con ese correo.");

        var sucursalId = esSuperAdmin ? dto.SucursalId : User.SucursalId();
        if (!sucursalId.HasValue)
            return BadRequest("Debes indicar una sucursal.");

        if (!await _context.Sucursales.AnyAsync(s => s.Id == sucursalId && s.Activa && s.EmpresaId == empresaId))
            return BadRequest("La sucursal indicada no existe o esta inactiva.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = email,
            Rol = dto.Rol,
            EmpresaId = empresaId,
            SucursalId = sucursalId
        };
        usuario.PasswordHash = _hasher.HashPassword(usuario, dto.Password);

        _context.Usuarios.Add(usuario);
        await _context.SaveChangesAsync();

        return StatusCode(StatusCodes.Status201Created, new { usuario.Id, usuario.Email });
    }

    [EnableRateLimiting("login")]
    [HttpPost("login")]
    public async Task<ActionResult<AuthRespuestaDto>> Login(LoginDto dto)
    {
        var email = dto.Email.Trim().ToLowerInvariant();

        var usuario = await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);

        if (usuario == null || !usuario.Activo)
        {
            await _auditService.RegistrarAsync("LoginFallido", "Usuario",
                detalles: $"Usuario inexistente o inactivo: {email}");
            return Unauthorized("Credenciales invalidas.");  
        }

        if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > DateTime.UtcNow)
        {
            await _auditService.RegistrarAsync("LoginFallido", "Usuario", usuario.Id.ToString(),
                "Intento con la cuenta bloqueada", usuario.Id, usuario.Email);
            return Unauthorized("Credenciales invalidas."); 
        }

        var resultado = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.Password);

        if (resultado == PasswordVerificationResult.Failed)
        {
            var maxIntentos = _config.GetValue("Lockout:MaxIntentos", 5);
            var minutos = _config.GetValue("Lockout:MinutosBloqueo", 15);
            var hastaBloqueo = DateTime.UtcNow.AddMinutes(minutos);

            await _context.Usuarios
                .Where(u => u.Id == usuario.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.BloqueadoHasta,
                        u => u.IntentosFallidos + 1 >= maxIntentos ? hastaBloqueo : u.BloqueadoHasta)
                    .SetProperty(u => u.IntentosFallidos,
                        u => u.IntentosFallidos + 1 >= maxIntentos ? 0 : u.IntentosFallidos + 1));

            var bloqueado = await _context.Usuarios
                .AnyAsync(u => u.Id == usuario.Id && u.BloqueadoHasta > DateTime.UtcNow);

            await _auditService.RegistrarAsync("LoginFallido", "Usuario", usuario.Id.ToString(),
                "Contraseña incorrecta", usuario.Id, usuario.Email);

            if (bloqueado)
            {
                await _context.RefreshTokens
                    .Where(r => r.UsuarioId == usuario.Id && r.FechaRenovacion == null)
                    .ExecuteUpdateAsync(s => s.SetProperty(r => r.FechaRenovacion, DateTime.UtcNow));

                await _auditService.RegistrarAsync("CuentaBloqueada", "Usuario", usuario.Id.ToString(),
                    usuarioId: usuario.Id, email: usuario.Email);
            }

            return Unauthorized("Credenciales invalidas.");
        }
        if (usuario.IntentosFallidos > 0 || usuario.BloqueadoHasta != null)
        {
            await _context.Usuarios
                .Where(u => u.Id == usuario.Id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.IntentosFallidos, 0)
                    .SetProperty(u => u.BloqueadoHasta, (DateTime?)null));
        }

        await _auditService.RegistrarAsync("LoginCorrecto", "Usuario", usuario.Id.ToString(),
            usuarioId: usuario.Id, email: usuario.Email);

        var (respuesta2, _) = await EmitirTokenAsync(usuario);
        return Ok(respuesta2);
    }

    [EnableRateLimiting("refresh")]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthRespuestaDto>> Refresh()
    {
        var refreshCookie = Request.Cookies[RefreshCookie];
        if (string.IsNullOrEmpty(refreshCookie))
            return Unauthorized("Refresh token invalido.");

        var hash = _tokenService.HashearToken(refreshCookie);
        var guardado = await _context.RefreshTokens
            .Include(r => r.Usuario)
            .FirstOrDefaultAsync(r => r.TokenHash == hash);

        if (guardado == null)
            return Unauthorized("Refresh token incalido");
        //El valor de FechaRenovacion indica que el token ya fue usado para renovar, por lo que no se puede volver a usar
        if (guardado.FechaRenovacion != null)
        {
            var activos = await _context.RefreshTokens
                .Where(r => r.UsuarioId == guardado.UsuarioId && r.FechaRenovacion == null)
                .ToListAsync();
            foreach (var r in activos)
                r.FechaRenovacion = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            await _auditService.RegistrarAsync("RefreshTokenReutilizado", "Usuario",
                guardado.UsuarioId.ToString(),
                "Se reutilizo un refresh token revocado; se cerraron todas las sesiones",
                guardado.UsuarioId, guardado.Usuario.Email);
            return Unauthorized("Refresh token invalido.");
        }

        if (guardado.FechaExpiracion <= DateTime.UtcNow || !guardado.Usuario.Activo)
            return Unauthorized("Refresh token invalido.");

        var (respuesta, nuevoHash) = await EmitirTokenAsync(guardado.Usuario);

        guardado.FechaRenovacion = DateTime.UtcNow;
        guardado.ReemplazadoPorHash = nuevoHash;
        await _context.SaveChangesAsync();

        return Ok(respuesta);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        var refreshCookie = Request.Cookies[RefreshCookie];
        if (!string.IsNullOrEmpty(refreshCookie))
        {
            var hash = _tokenService.HashearToken(refreshCookie);

            var guardado = await _context.RefreshTokens
                .FirstOrDefaultAsync(r => r.TokenHash == hash && r.FechaRenovacion == null);

            if (guardado != null)
            {
                guardado.FechaRenovacion = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        Response.Cookies.Delete(AccessCookie, OpcionesCookie("/"));
        Response.Cookies.Delete(RefreshCookie, OpcionesCookie(RefreshPath));
        return NoContent();
    }

    [Authorize(Roles = "SuperAdmin")]
    [EnableRateLimiting("login")]
    [HttpPost("cambiar-password")]
    public async Task<IActionResult> CambiarPassword(CambiarPasswordDto dto)
    {
        var idClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(idClaim, out var usuarioId))
            return Unauthorized();

        var usuario = await _context.Usuarios.FindAsync(usuarioId);
        if (usuario == null || !usuario.Activo)
            return Unauthorized();

        var verificacion = _hasher.VerifyHashedPassword(usuario, usuario.PasswordHash, dto.PasswordActual);
        if (verificacion == PasswordVerificationResult.Failed)
            return BadRequest("La contraseña actual es incorrecta.");

        if (dto.PasswordActual == dto.PasswordNueva)
            return BadRequest("La contraseña nueva debe de ser distinta de la actual.");

        usuario.PasswordHash = _hasher.HashPassword(usuario, dto.PasswordNueva);
        await _context.SaveChangesAsync();

        await _context.RefreshTokens
            .Where(r => r.UsuarioId == usuarioId && r.FechaRenovacion == null)
            .ExecuteUpdateAsync(s => s.SetProperty(r => r.FechaRenovacion, DateTime.UtcNow));

        return NoContent();
    }

    private const string AccessCookie = "access_token";
    private const string RefreshCookie = "refresh_token";
    private const string RefreshPath = "/api/auth";

    private static CookieOptions OpcionesCookie(string path, DateTime? expira = null) => new()
    {
        HttpOnly = true,
        Secure = true,
        SameSite = SameSiteMode.Strict,
        Path = path,
        Expires = expira
    };

    private async Task<(AuthRespuestaDto Respuesta, string RefreshHash)> EmitirTokenAsync(Usuario usuario)
    {
        var (refresh, hash, expira) = _tokenService.GenerarRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UsuarioId = usuario.Id,
            TokenHash = hash,
            FechaExpiracion = expira
        });
        await _context.SaveChangesAsync();

        var minutos = _config.GetValue("Jwt:ExpireMinutes", 60);
        Response.Cookies.Append(AccessCookie, _tokenService.GenerarToken(usuario),
            OpcionesCookie("/", DateTime.UtcNow.AddMinutes(minutos)));
        Response.Cookies.Append(RefreshCookie, refresh, OpcionesCookie(RefreshPath, expira));

        return (new AuthRespuestaDto
        {
            Nombre = usuario.Nombre,
            Rol = usuario.Rol,
            SucursalId = usuario.SucursalId
        }, hash);
    }
}
