using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class ProductoLote
    {
        public int Id { get; set; }
        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        [Required]
        public string NumeroLote { get; set; } = string.Empty;
        public DateOnly? FechaFabricacion { get; set; }
        public DateOnly? FechaCaducidad { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
