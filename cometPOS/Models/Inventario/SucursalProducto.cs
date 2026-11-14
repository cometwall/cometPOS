using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class SucursalProducto
    {
        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;

        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        [Column(TypeName = "numeric(18,4)")]
        public decimal? PrecioVenta { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal StockMinimo { get; set; } = 0;
        [Column(TypeName = "numeric(18,4)")]
        public decimal? StockMaximo { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal? PuntoReorden { get; set; }

        public string? Ubicacion { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public InventarioSucursal? Inventario { get; set; }
    }
}
