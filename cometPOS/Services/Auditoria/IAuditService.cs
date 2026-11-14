namespace cometPOS.Services
{
    public interface IAuditService
    {
        Task RegistrarAsync(string accion, string entidad, string? entidadId = null,
            string? detalles = null, int? usuarioId = null, string? email = null);
    }
}
