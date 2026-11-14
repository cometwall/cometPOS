namespace cometPOS.Models
{
    public class ProductoAtributo
    {
        public int ProductoId { get; set; }
        public Producto Producto { get; set; } = null!;

        public int AtributoId { get; set; }
        public Atributo Atributo { get; set; } = null!;

        public int Orden { get; set; } = 0;
    }
}
