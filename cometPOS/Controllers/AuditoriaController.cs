using cometPOS.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using cometPOS.Services;

namespace cometPOS.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "SuperAdmin")]
    public class AuditoriaController : ControllerBase
    {
        private readonly AppDbContext _context;

        public AuditoriaController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Listar(
            [FromQuery] string? entidad,
            [FromQuery] string? entidadId,
            [FromQuery] int? usuarioId,
            [FromQuery] string? accion,
            [FromQuery] DateTime? desde,
            [FromQuery] DateTime? hasta,
            [FromQuery] int pagina = 1,
            [FromQuery] int tamano = 50)
        {
            pagina = Math.Max(1, pagina);
            tamano = Math.Clamp(tamano, 1, 200);

            var empresaId = User.EmpresaId() ?? 0;
            var query = _context.AuditLogs.AsNoTracking()
                .Where(a => a.EmpresaId == empresaId);

            if (!string.IsNullOrWhiteSpace(entidad)) query = query.Where(a => a.Entidad == entidad);
            if (!string.IsNullOrWhiteSpace(entidadId)) query = query.Where(a => a.EntidadId == entidadId);
            if (usuarioId.HasValue) query = query.Where(a => a.UsuarioId == usuarioId);
            if (!string.IsNullOrWhiteSpace(accion)) query = query.Where(a => a.Accion == accion);
            if (desde.HasValue) query = query.Where(a => a.FechaUtc >= desde.Value.ToUniversalTime());
            if (hasta.HasValue) query = query.Where(a => a.FechaUtc <= hasta.Value.ToUniversalTime());

            var total = await query.CountAsync();

            var items = await query
                .OrderByDescending(a => a.FechaUtc)
                .Skip((pagina - 1) * tamano)
                .Take(tamano)
                .ToListAsync();

            return Ok(new { total, pagina, tamano, items });
        }
    }
}
