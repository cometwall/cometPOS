using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class IdentificadorProducto
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        [Required]
        public string Tipo { get; set; } = "EAN13";
        [Required]
        public string Codigo { get; set; } = string.Empty;

        public int? UnidadId { get; set; }
        public UnidadMedida? Unidad { get; set; }

        public bool EsPrincipal { get; set; } = false;
        public bool Activo { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
