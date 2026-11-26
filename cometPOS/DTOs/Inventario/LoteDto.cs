namespace cometPOS.DTOs.Inventario
{
    public class LoteDto
    {
        public int Id { get; set; }
        public int VarianteId { get; set; } 
        public string NumeroLote { get; set; } = string.Empty;
        public DateOnly? FechaFabricacion { get; set; }
        public DateOnly? FechaCaducidad { get; set; }
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
    }
}
