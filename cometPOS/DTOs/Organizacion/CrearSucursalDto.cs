using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class CrearSucursalDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Codigo { get; set; } = string.Empty;
        [Required]
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;
    }
}
