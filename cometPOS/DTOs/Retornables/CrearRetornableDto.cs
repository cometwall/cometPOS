using System.ComponentModel.DataAnnotations;
namespace cometPOS.DTOs.Retornables
{
    public class CrearRetornableDto
    {
        [Required]
        public string Codigo { get; set; } = string.Empty;
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public decimal ValorDeposito { get; set; } = 0;
        public decimal Costo { get; set; } = 0;
        public bool EsPropio { get; set; } = true;
    }
}
