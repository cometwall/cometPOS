using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class Sucursal
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Codigo { get; set; } = string.Empty;
        [Required]
        public string Direccion { get; set; } = string.Empty;

        public string? Telefono { get; set; }
        public bool Activa { get; set; }

        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
