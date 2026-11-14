using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class InventarioRetornable
    {
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;

        public int RetornableId { get; set; }
        public Retornable Retornable { get; set; } = null!;

        [Column(TypeName = "numeric(18,4)")]
        public decimal CantidadVacios { get; set; } = 0;

        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}
