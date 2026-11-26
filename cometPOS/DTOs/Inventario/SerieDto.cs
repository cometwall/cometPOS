namespace cometPOS.DTOs.Inventario
{
    public class SerieDto
    {
        public int Id { get; set; }
        public int VarianteId { get; set; }
        public string NumeroSerie { get; set; } = string.Empty;
        public int? LoteId { get; set; }
        public int? SucursalId { get; set; }
        public string Estado { get; set; } = string.Empty;
        public DateTime FechaCreacion { get; set; }
    }
}
