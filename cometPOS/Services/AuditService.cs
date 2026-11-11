using System.Security.Claims;
using System.Text.Json;
using cometPOS.Data;
using cometPOS.Models;
using Microsoft.EntityFrameworkCore;

namespace cometPOS.Services
{
    public class AuditService : IAuditService
    {
        private readonly AppDbContext _context;
        private readonly IHttpContextAccessor _http;

        public AuditService(AppDbContext context, IHttpContextAccessor http)
        {
            _context = context;
            _http = http;
        }

        public async Task RegistrarAsync(string accion, string entidad, string? entidadId = null,
            string? detalles = null, int? usuarioId = null, string? email = null)
        {
            var ctx = _http.HttpContext;

            if (usuarioId == null && int.TryParse(ctx?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var id))
                usuarioId = id;

            email ??= ctx?.User.FindFirstValue(ClaimTypes.Email);

            var empresaId = ctx?.User.EmpresaId();
            if (empresaId == null && usuarioId != null)
                empresaId = await _context.Usuarios
                    .Where(u => u.Id == usuarioId)
                    .Select(u => (int?)u.EmpresaId)
                    .FirstOrDefaultAsync();

            _context.AuditLogs.Add(new AuditLog
            {
                UsuarioId = usuarioId,
                EmpresaId = empresaId,
                UsuarioEmail = email,
                Ip = ctx?.Connection.RemoteIpAddress?.ToString(),
                Accion = accion,
                Entidad = entidad,
                EntidadId = entidadId,
                Detalles = detalles is null ? null : JsonSerializer.Serialize(new { mensaje = detalles })
            });

            await _context.SaveChangesAsync();
        }
    }
}