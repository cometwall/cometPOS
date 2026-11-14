namespace cometPOS.DTOs.Catalogo
{
    public class CategoriaDto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public int? CategoriaPadreId { get; set; }
        public string Nombre { get; set; } = string.Empty;
    }
}
