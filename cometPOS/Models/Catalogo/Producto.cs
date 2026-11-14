using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class Producto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        [Required]
        public string Nombre { get; set; } = string.Empty;
        public string? Descripcion { get; set; }

        public int? CategoriaId { get; set; }
        public Categoria? Categoria { get; set; }
        public int? MarcaId { get; set; }
        public Marca? Marca { get; set; }

        public int UnidadBaseId { get; set; }
        public UnidadMedida UnidadBase { get; set; } = null!;

        [Required]
        public string Tipo { get; set; } = "PRODUCTO"; // PRODUCTO | SERVICIO
        public bool ControlaInventario { get; set; } = true;
        [Required]
        public string TipoSeguimiento { get; set; } = "NINGUNO"; // NINGUNO | LOTE | SERIE
        public string? ClaveSatProdServ { get; set; }

        [Column(TypeName = "jsonb")]
        public string? Especificaciones { get; set; }

        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
        public DateTime? FechaActualizacion { get; set; }

        public ICollection<ProductoVariante> Variantes { get; set; } = new List<ProductoVariante>();
        public ICollection<ProductoAtributo> Atributos { get; set; } = new List<ProductoAtributo>();
    }
}
