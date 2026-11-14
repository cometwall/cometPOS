using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class RegistrarUsuarioDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string Password { get; set; } = string.Empty;
        [Required]
        public string Rol { get; set; } = "Cajero"; // Valor por defecto
        public int? SucursalId { get; set; } // Nullable para permitir que sea opcional
    }
}
