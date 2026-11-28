namespace cometPOS.DTOs.Retornables
{
    public class InventarioRetornableDto
    {
        public int SucursalId { get; set; }
        public int RetornableId { get; set; }
        public string RetornableCodigo { get; set; } = string.Empty;
        public string RetornableNombre { get; set; } = string.Empty;
        public decimal CantidadVacios { get; set; }
        public DateTime FechaActualizacion { get; set; }
    }
}
