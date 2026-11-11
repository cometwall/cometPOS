using System.Runtime.CompilerServices;
using System.Security.Claims;
using System.Text.Json;
using cometPOS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace cometPOS.Data
{
    public class AuditInterceptor : SaveChangesInterceptor
    {
        private static readonly HashSet<Type> TiposExcluidos = new()
        {
            typeof(AuditLog),
            typeof(RefreshToken)
        };

        private static readonly HashSet<string> CamposSensibles = new()
        {
            "PasswordHash",
            "TokenHash"
        };

        private sealed record Pendiente(EntityEntry Entrada, string Accion, string Detalles);

        private readonly IHttpContextAccessor _http;
        private readonly ConditionalWeakTable<DbContext, List<Pendiente>> _pendientes = new();

        public AuditInterceptor(IHttpContextAccessor http)
        {
            _http = http;
        }

        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
            DbContextEventData eventData, InterceptionResult<int> result,
            CancellationToken cancellationToken = default)
        {
            if (eventData.Context != null)
                Capturar(eventData.Context);

            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }

        public override async ValueTask<int> SavedChangesAsync(
            SaveChangesCompletedEventData eventData, int result,
            CancellationToken cancellationToken = default)
        {
            var context = eventData.Context;

            if (context != null && _pendientes.TryGetValue(context, out var lista))
            {
                _pendientes.Remove(context);

                if (lista.Count > 0)
                {
                    var (usuarioId, email, ip, empresaActor) = ObtenerActor();

                    foreach (var p in lista)
                    {
                        context.Set<AuditLog>().Add(new AuditLog
                        {
                            UsuarioId = usuarioId,
                            EmpresaId = ObtenerEmpresa(p.Entrada) ?? empresaActor,
                            UsuarioEmail = email,
                            Ip = ip,
                            Accion = p.Accion,
                            Entidad = p.Entrada.Metadata.ClrType.Name,
                            EntidadId = ObtenerClave(p.Entrada),
                            Detalles = p.Detalles
                        });
                    }

                    await context.SaveChangesAsync(cancellationToken);
                }
            }

            return await base.SavedChangesAsync(eventData, result, cancellationToken);
        }

        private void Capturar(DbContext context)
        {
            var lista = new List<Pendiente>();

            foreach (var entrada in context.ChangeTracker.Entries())
            {
                if (TiposExcluidos.Contains(entrada.Metadata.ClrType))
                    continue;

                switch (entrada.State)
                {
                    case EntityState.Added:
                        lista.Add(new Pendiente(entrada, "Crear", Serializar(entrada, valoresNuevos: true)));
                        break;
                    case EntityState.Modified:
                        lista.Add(new Pendiente(entrada, "Modificar", SerializarCambios(entrada)));
                        break;
                    case EntityState.Deleted:
                        lista.Add(new Pendiente(entrada, "Eliminar", Serializar(entrada, valoresNuevos: false)));
                        break;
                }
            }

            if (lista.Count > 0)
                _pendientes.AddOrUpdate(context, lista);
        }

        private static string Serializar(EntityEntry entrada, bool valoresNuevos)
        {
            var datos = new Dictionary<string, object?>();

            foreach (var prop in entrada.Properties)
            {
                var nombre = prop.Metadata.Name;
                var valor = valoresNuevos ? prop.CurrentValue : prop.OriginalValue;
                datos[nombre] = CamposSensibles.Contains(nombre) ? "[OCULTO]" : valor;
            }

            return JsonSerializer.Serialize(datos);
        }

        private static string SerializarCambios(EntityEntry entrada)
        {
            var cambios = new Dictionary<string, object?>();

            foreach (var prop in entrada.Properties.Where(p => p.IsModified))
            {
                var nombre = prop.Metadata.Name;

                if (Equals(prop.OriginalValue, prop.CurrentValue))
                    continue;

                cambios[nombre] = CamposSensibles.Contains(nombre)
                    ? new { antes = "[OCULTO]", despues = "[OCULTO]" }
                    : new { antes = prop.OriginalValue, despues = prop.CurrentValue };
            }

            return JsonSerializer.Serialize(cambios);
        }

        private static string? ObtenerClave(EntityEntry entrada)
        {
            var clave = entrada.Metadata.FindPrimaryKey();
            if (clave == null)
                return null;

            return string.Join(",", clave.Properties
                .Select(p => entrada.Property(p.Name).CurrentValue?.ToString()));
        }

        private static int? ObtenerEmpresa(EntityEntry entrada)
        {
            var prop = entrada.Properties.FirstOrDefault(p => p.Metadata.Name == "EmpresaId");
            return prop?.CurrentValue as int?;
        }

        private (int? UsuarioId, string? Email, string? Ip, int? EmpresaId) ObtenerActor()
        {
            var ctx = _http.HttpContext;
            if (ctx == null)
                return (null, null, null, null);

            int? id = int.TryParse(ctx.User.FindFirstValue(ClaimTypes.NameIdentifier), out var v) ? v : null;
            var email = ctx.User.FindFirstValue(ClaimTypes.Email)
                        ?? ctx.User.FindFirstValue("email");
            int? empresaId = int.TryParse(ctx.User.FindFirstValue("EmpresaId"), out var e) ? e : null;

            return (id, email, ctx.Connection.RemoteIpAddress?.ToString(), empresaId);
        }
    }
}