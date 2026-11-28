namespace cometPOS.DTOs.Retornables
{
    public class MovimientoRetornableDto
    {
        public long Id { get; set; }
        public int SucursalId { get; set; }
        public string SucursalNombre { get; set; } = string.Empty;
        public int RetornableId { get; set; }
        public string RetornableNombre { get; set; } = string.Empty;
        public string Tipo { get; set; } = string.Empty;
        public decimal Cantidad { get; set; }
        public decimal? ValorDeposito { get; set; }
        public string? ReferenciaTipo { get; set; }
        public long? ReferenciaId { get; set; }
        public int? UsuarioId { get; set; }
        public string? UsuarioNombre { get; set; }
        public string? Notas { get; set; }
        public DateTime Fecha { get; set; }
    }
}
