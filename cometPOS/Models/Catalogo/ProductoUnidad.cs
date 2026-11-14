using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class ProductoUnidad
    {
        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        public int UnidadId { get; set; }
        public UnidadMedida Unidad { get; set; } = null!;

        [Column(TypeName = "numeric(18,6)")]
        public decimal Factor { get; set; }

        public bool EsVenta { get; set; } = true;
        public bool EsCompra { get; set; } = true;
    }
}
