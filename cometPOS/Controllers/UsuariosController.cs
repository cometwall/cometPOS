using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using cometPOS.Data;
using cometPOS.DTOs;
using System.Linq.Expressions;
using cometPOS.Models;
using System.Linq.Expressions;

namespace cometPOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Admin")]
    public class UsuariosController : ControllerBase
    {
        private readonly AppDbContext _context;
        private static readonly Expression<Func<Usuario, UsuarioDto>> AUsuarioDto = u => new UsuarioDto
        {
            Id = u.Id,
            Nombre = u.Nombre,
            Email = u.Email,
            Rol = u.Rol,
            SucursalId = u.SucursalId,
            Activo = u.Activo,
            BloqueadoHasta = u.BloqueadoHasta,
            FechaCreacion = u.FechaCreacion
        };

        private Task<int> RevocarRefreshTokensAsync(int usuarioId) =>
            _context.RefreshTokens
                .Where(r => r.UsuarioId == usuarioId && r.FechaRenovacion == null)
                .ExecuteUpdateAsync(s => s.SetProperty(r => r.FechaRenovacion, DateTime.UtcNow));

        private static readonly string[] RolesValidos = { "Admin", "Cajero" };

        public UsuariosController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost("{id}/desbloquear")]
        public async Task<IActionResult> Desbloquear(int id)
        {
            var filas = await _context.Usuarios
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s
                    .SetProperty(u => u.IntentosFallidos, 0)
                    .SetProperty(u => u.BloqueadoHasta, (DateTime?)null));
            return filas == 0 ? NotFound() : NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<UsuarioDto>>> Listar([FromQuery] bool incluirInactivos = false)
        {
            var query = _context.Usuarios.AsQueryable();

            if (!incluirInactivos)
                query = query.Where(u => u.Activo);

            var usuarios = await query
                .OrderBy(u => u.Nombre)
                .Select(AUsuarioDto)
                .ToListAsync();
            return Ok(usuarios);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<UsuarioDto>> ObtenerPorId(int id)
        {
            var usuario = await _context.Usuarios
                .Where(u => u.Id == id)
                .Select(AUsuarioDto)
                .FirstOrDefaultAsync();
            return usuario == null ? NotFound() : Ok(usuario);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Actualizar(int id, ActualizarUsuarioDto dto)
        {
            if (!RolesValidos.Contains(dto.Rol))
                return BadRequest($"Rol invalido. Valores permitidos: {string.Join(",", RolesValidos)}.");

            if (dto.SucursalId.HasValue &&
                !await _context.Sucursales.AnyAsync(s => s.Id == dto.SucursalId && s.Activa))
                return BadRequest("La sucursal indicada no existe o esta inactiva.");

            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Nombre = dto.Nombre;
            usuario.Rol = dto.Rol;
            usuario.SucursalId = dto.SucursalId;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPost("{id}/desactivar")]
        public async Task<IActionResult> Desactivar(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario == null)
                return NotFound();

            usuario.Activo = false;
            await _context.SaveChangesAsync();
            await RevocarRefreshTokensAsync(id);

            return NoContent();
        }

        [HttpPost("{id}/activar")]
        public async Task<IActionResult> Activar(int id)
        {
            var filas = await _context.Usuarios
                .Where(u => u.Id == id)
                .ExecuteUpdateAsync(s => s.SetProperty(u => u.Activo, true));

            return filas == 0 ? NotFound() : NoContent();
        }
    }
}
