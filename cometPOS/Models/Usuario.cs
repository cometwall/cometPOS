using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Email { get; set; } = string.Empty;
        [Required]
        public string PasswordHash { get; set; } = string.Empty;
        [Required]
        public string Rol { get; set; } = "Cajero"; // Cajero, Administrador, Gerente, etc.
        
        public int? SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
