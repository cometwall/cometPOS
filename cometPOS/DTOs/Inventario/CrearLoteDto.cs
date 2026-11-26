using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Inventario
{
    public class CrearLoteDto
    {
        [Required]
        public int VarianteId { get; set; }
        [Required]
        public string NumeroLote { get; set; } = string.Empty;
        public DateOnly? FechaFabricacion { get; set; }
        public DateOnly? FechaCaducidad { get; set; }
    }
}
