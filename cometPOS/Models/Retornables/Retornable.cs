using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class Retornable
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        [Required]
        public string Codigo { get; set; } = string.Empty;
        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        [Column(TypeName = "numeric(18,4)")]
        public decimal ValorDeposito { get; set; } = 0;
        [Column(TypeName = "numeric(18,4)")]
        public decimal Costo { get; set; } = 0;

        public bool EsPropio { get; set; } = true;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
