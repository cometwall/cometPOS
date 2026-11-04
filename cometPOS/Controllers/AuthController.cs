using Microsoft.AspNetCore.Mvc;
using cometPOS.Data;
using cometPOS.DTOs;
using cometPOS.Models;
using cometPOS.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.RateLimiting;

namespace cometPOS.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private static readonly string[] RolesValidos = { "Admin", "Cajero" };
    private readonly AppDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IConfiguration _config;
    private readonly PasswordHasher<Usuario> _hasher = new();
    public AuthController(AppDbContext context, ITokenService tokenService, IConfiguration config)
    {
        _context = context;
        _tokenService = tokenService;
        _config = config;
    }
    
    [Authorize(Roles = "Admin")]
    [HttpPost("registro")]
    public async Task<ActionResult> Registrar(RegistrarUsuarioDto dto)
    {
        if (!RolesValidos.Contains(dto.Rol))
            return BadRequest($"Rol invalido. Valores permitidos: {string.Join(", ", RolesValidos)}.");

        var email = dto.Email.Trim().ToLowerInvariant();

        if (await _context.Usuarios.AnyAsync(u => u.Email == email))
            return Conflict("Ya existe un usuario con ese correo.");

        if (dto.SucursalId.HasValue &&
            !await _context.Sucursales.AnyAsync(s => s.Id == dto.SucursalId && s.Activa))
            return BadRequest("La sucursal indicada no existe o esta inactiva.");

        var usuario = new Usuario
        {
            Nombre = dto.Nombre,
            Email = email,
            Rol = dto.Rol,
            SucursalId = dto.SucursalId
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
            return Unauthorized("Credenciales invalidas.");

        if (usuario.BloqueadoHasta.HasValue && usuario.BloqueadoHasta > DateTime.UtcNow)
            return Unauthorized("Credenciales invalidas.");

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

            if (bloqueado)
            {
                await _context.RefreshTokens
                    .Where(r => r.UsuarioId == usuario.Id && r.FechaRenovacion == null)
                    .ExecuteUpdateAsync(s => s.SetProperty(r => r.FechaRenovacion, DateTime.UtcNow));
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

        return Ok(await EmitirTokenAsync(usuario));
    }

    [EnableRateLimiting("refresh")]
    [HttpPost("refresh")]
    public async Task<ActionResult<AuthRespuestaDto>> Refresh(RefreshTokenDto dto)
    {
        var hash = _tokenService.HashearToken(dto.RefreshToken);
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
            return Unauthorized("Refresh token invalido.");
        }

        if (guardado.FechaExpiracion <= DateTime.UtcNow || !guardado.Usuario.Activo)
            return Unauthorized("Refresh token invalido.");

        var respuesta = await EmitirTokenAsync(guardado.Usuario);

        guardado.FechaRenovacion = DateTime.UtcNow;
        guardado.ReemplazadoPorHash = _tokenService.HashearToken(respuesta.RefreshToken);
        await _context.SaveChangesAsync();

        return Ok(respuesta);
    }

    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto dto)
    {
        var hash = _tokenService.HashearToken(dto.RefreshToken);

        var guardado = await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.TokenHash == hash && r.FechaRenovacion == null);

        if (guardado != null)
        {
            guardado.FechaRenovacion = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
        return NoContent();
    }

    private async Task<AuthRespuestaDto> EmitirTokenAsync(Usuario usuario)
    {
        var (refresh, hash, expira) = _tokenService.GenerarRefreshToken();

        _context.RefreshTokens.Add(new RefreshToken
        {
            UsuarioId = usuario.Id,
            TokenHash = hash,
            FechaExpiracion = expira
        });
        await _context.SaveChangesAsync();

        return new AuthRespuestaDto
        {
            Token = _tokenService.GenerarToken(usuario),
            RefreshToken = refresh,
            Nombre = usuario.Nombre,
            Rol = usuario.Rol,
            SucursalId = usuario.SucursalId
        };
    }
}
