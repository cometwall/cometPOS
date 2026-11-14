namespace cometPOS.Models
{
    public class VarianteValor
    {
        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        public int AtributoId { get; set; }
        public Atributo Atributo { get; set; } = null!;

        public int AtributoValorId { get; set; }
        public AtributoValor AtributoValor { get; set; } = null!;
    }
}
