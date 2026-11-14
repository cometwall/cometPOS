using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public int? CategoriaPadreId { get; set; }
        public Categoria? CategoriaPadre { get; set; }
        public ICollection<Categoria> Subcategorias { get; set; } = new List<Categoria>();

        [Required]
        public string Nombre { get; set; } = string.Empty;
        public bool Activa { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
