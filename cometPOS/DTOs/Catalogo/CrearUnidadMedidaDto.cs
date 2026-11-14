using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Catalogo
{
    public class CrearUnidadMedidaDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Abreviatura { get; set; } = string.Empty;
        public string? ClaveSat { get; set; }
        public bool PermiteDecimales { get; set; } = false;

    }
}
