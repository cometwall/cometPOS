using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class ProductoVariante
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        [Required]
        public string Sku { get; set; } = string.Empty;
        public string? Nombre { get; set; }
        public bool EsDefault { get; set; } = false;

        [Column(TypeName = "numeric(18,4)")]
        public decimal PrecioVenta { get; set; } = 0;

        public bool Activa { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }
    }
}
