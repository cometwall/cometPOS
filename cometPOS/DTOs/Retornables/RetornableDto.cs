namespace cometPOS.DTOs.Retornables
{
    public class RetornableDto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal ValorDeposito { get; set; } = 0;
        public decimal Costo { get; set; } = 0;
        public bool EsPropio { get; set; } = true;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
