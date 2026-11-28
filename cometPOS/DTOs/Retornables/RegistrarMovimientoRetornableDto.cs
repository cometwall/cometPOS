using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Retornables
{
    public class RegistrarMovimientoRetornableDto
    {
        [Required]
        public int SucursalId { get; set; }
        [Required]
        public int RetornableId { get; set; }
        [Required]
        public string Tipo { get; set; } = string.Empty;
        [Range(0.0001, double.MaxValue)]
        public decimal Cantidad { get; set; }
        public decimal? ValorDeposito { get; set; }
        public string? Notas { get; set; }
    }
}
