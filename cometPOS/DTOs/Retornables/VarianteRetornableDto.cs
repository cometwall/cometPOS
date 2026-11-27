namespace cometPOS.DTOs.Retornables
{
    public class VarianteRetornableDto
    {
        public int VarianteId { get; set; }
        public int RetornableId { get; set; }
        public string RetornableCodigo { get; set; } = string.Empty;
        public string RetornableNombre { get; set; } = string.Empty;
        public decimal CantidadPorUnidad { get; set; }
    }
}
