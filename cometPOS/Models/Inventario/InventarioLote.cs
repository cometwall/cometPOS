using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class InventarioLote
    {
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;

        public int LoteId { get; set; }
        public ProductoLote Lote { get; set; } = null!;

        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        [Column(TypeName = "numeric(18,4)")]
        public decimal Cantidad { get; set; } = 0;

        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}
