namespace cometPOS.DTOs.Inventario
{
    public class SucursalProductoDto
    {
        public int VarianteId { get; set; }
        public int SucursalId { get; set; }
        public decimal? PrecioVenta { get; set; }
        public decimal StockMinimo { get; set; }
        public decimal StockMaximo { get; set; }
        public decimal PuntoReorden { get; set; }
        public string? Ubicacion { get; set; }
        public decimal Cantidad { get; set; }
    }
}
