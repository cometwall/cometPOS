using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Retornables
{
    public class AsignarVarianteRetornableDto
    {
        [Required]
        public int VarianteId { get; set; }
        [Required]
        public int RetornableId { get; set; }
        [Range(0.0001, double.MaxValue)]
        public decimal CantidadPorUnidad { get; set; } = 1;
    }
}
