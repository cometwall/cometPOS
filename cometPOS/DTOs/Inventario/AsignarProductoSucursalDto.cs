using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Inventario
{
    public class AsignarProductoSucursalDto
    {
        [Required]
        public int VarianteId { get; set; }
        [Required]
        public int SucursalId { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal StockMinimo { get; set; } = 0;
        public decimal? StockMaximo { get; set; }
        public decimal? PuntoReorden { get; set; }
        public string? Ubicacion { get; set; }
    }
}
