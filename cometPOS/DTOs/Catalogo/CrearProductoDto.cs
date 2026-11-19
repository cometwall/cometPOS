using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Catalogo
{
    public class CrearProductoDto
    {
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; } = string.Empty;
        public int? CategoriaId { get; set; }
        public int? MarcaId { get; set; }
        [Required]
        public int UnidadBaseId { get; set; }
        [Required]
        public string Sku { get; set; } = string.Empty;
        [Required]
        public decimal PrecioVenta { get; set; } = 0;
    }
}
