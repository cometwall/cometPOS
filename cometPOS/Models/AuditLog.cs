namespace cometPOS.Models
{
    public class AuditLog
    {
        public long Id { get; set; }
        public DateTime FechaUtc { get; set; } = DateTime.UtcNow;

        public int? UsuarioId { get; set; }
        public int? EmpresaId { get; set; }
        public string? UsuarioEmail { get; set; }
        public string? Ip { get; set; }

        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public string? EntidadId { get; set; }

        public string? Detalles { get; set; }
    }
}
