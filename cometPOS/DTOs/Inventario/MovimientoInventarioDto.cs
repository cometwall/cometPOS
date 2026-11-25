namespace cometPOS.DTOs.Inventario
{
    public class MovimientoInventarioDto
    {
        public long Id { get; set; }
        public int SucursalId { get; set; }
        public int VarianteId { get; set; }
        public string Tipo { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public string? Notas { get; set; }
        public DateTime FechaMovimiento { get; set; }
        public decimal CantidadResultante { get; set; }
    }
}
