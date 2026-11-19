namespace cometPOS.DTOs.Catalogo
{
    public class ProductoDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }
        public int? CategoriaId { get; set; }
        public int? MarcaId { get; set; }
        public string? Sku { get; set; } = string.Empty;
        public decimal PrecioVenta { get; set; }
    }
}
