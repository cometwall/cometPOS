using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class InventarioSucursal
    {
        public int SucursalId { get; set; }
        public int VarianteId { get; set; }
        public SucursalProducto SucursalProducto { get; set; } = null!;

        [Column(TypeName = "numeric(18,4)")]
        public decimal Cantidad { get; set; } = 0;
        [Column(TypeName = "numeric(18,4)")]
        public decimal CantidadReservada { get; set; } = 0;
        [Column(TypeName = "numeric(18,4)")]
        public decimal CostoPromedio { get; set; } = 0;

        public DateTime FechaActualizacion { get; set; } = DateTime.UtcNow;
    }
}
