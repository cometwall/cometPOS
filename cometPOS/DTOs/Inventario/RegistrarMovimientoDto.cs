using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Inventario
{
    public class RegistrarMovimientoDto
    {
        [Required]
        public int SucursalId { get; set; }
        [Required]
        public int VarianteId { get; set; }
        [Required]
        public string Tipo { get; set; } = string.Empty;
        [Required]
        [Range(typeof(decimal), "0.0001", "79228162514264337593543950335" ,
            ErrorMessage = "La cantidad debe ser mayor a 0")]
        public decimal Cantidad { get; set; }
        public decimal? CostoUnitario { get; set; }
        public string? Notas { get; set; }
    }
}
