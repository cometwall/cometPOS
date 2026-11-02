namespace cometPOS.Models
{
    public class RefreshToken
    {
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public Usuario Usuario { get; set; } = null!;
        public string TokenHash { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime FechaExpiracion { get; set; }
        public DateTime? FechaRenovacion { get; set; }
        public string? ReemplazadoPorHash { get; set; }
        public bool EstaActivo => FechaRenovacion == null && FechaExpiracion > DateTime.UtcNow;
    }
}
