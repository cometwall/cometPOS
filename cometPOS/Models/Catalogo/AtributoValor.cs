using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class AtributoValor
    {
        public int Id { get; set; }
        public int AtributoId { get; set; }
        public Atributo Atributo { get; set; } = null!;

        [Required]
        public string Valor { get; set; } = string.Empty;
        public int Orden { get; set; } = 0;
    }
}
