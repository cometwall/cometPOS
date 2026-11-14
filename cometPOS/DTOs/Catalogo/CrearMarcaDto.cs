using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Catalogo
{
    public class CrearMarcaDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
    }
}
