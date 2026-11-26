using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Inventario
{
    public class CrearSerieDto
    {
        [Required]
        public int VarianteId { get; set; }
        [Required]
        public string NumeroSerie { get; set; } = string.Empty;
        public int? LoteId { get; set; }
    }
}
