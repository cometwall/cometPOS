using System.ComponentModel.DataAnnotations;
namespace cometPOS.DTOs.Catalogo
{
    public class CrearCategoriaDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public int? CategoriaPadreId { get; set; }
    }
}
