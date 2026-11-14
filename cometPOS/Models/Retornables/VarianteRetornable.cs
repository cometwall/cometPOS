using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class VarianteRetornable
    {
        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        public int RetornableId { get; set; }
        public Retornable Retornable { get; set; } = null!;

        [Column(TypeName = "numeric(18,4)")]
        public decimal CantidadPorUnidad { get; set; } = 1;
    }
}
