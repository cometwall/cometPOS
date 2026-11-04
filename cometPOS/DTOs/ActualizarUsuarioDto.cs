using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class ActualizarUsuarioDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Rol { get; set; } = string.Empty;
        public int? SucursalId { get; set; }
    }
}
