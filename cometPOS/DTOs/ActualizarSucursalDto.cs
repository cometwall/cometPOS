using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class ActualizarSucursalDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activa { get; set; }
    }
}
