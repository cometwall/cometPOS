using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class Atributo
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        [Required]
        public string Nombre { get; set; } = string.Empty;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
