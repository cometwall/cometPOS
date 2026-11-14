namespace cometPOS.DTOs
{
    public class UsuarioDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? SucursalId { get; set; }
        public bool Activo { get; set; }
        public DateTime? BloqueadoHasta { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
