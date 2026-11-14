using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class ProductoSerie
    {
        public int Id { get; set; }
        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        [Required]
        public string NumeroSerie { get; set; } = string.Empty;

        public int? LoteId { get; set; }
        public ProductoLote? Lote { get; set; }
        public int? SucursalId { get; set; }
        public Sucursal? Sucursal { get; set; }

        [Required]
        public string Estado { get; set; } = "DISPONIBLE";
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
