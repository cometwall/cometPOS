using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace cometPOS.Models
{
    public class MovimientoInventario
    {
        public long Id { get; set; }

        public int SucursalId { get; set; }
        public Sucursal Sucursal { get; set; } = null!;

        public int VarianteId { get; set; }
        public ProductoVariante Variante { get; set; } = null!;

        public int? LoteId { get; set; }
        public ProductoLote? Lote { get; set; }
        public int? SerieId { get; set; }
        public ProductoSerie? Serie { get; set; }

        [Required]
        public string Tipo { get; set; } = string.Empty;

        [Column(TypeName = "numeric(18,4)")]
        public decimal Cantidad { get; set; }
        [Column(TypeName = "numeric(18,4)")]
        public decimal? CostoUnitario { get; set; }

        public string? ReferenciaTipo { get; set; }
        public long? ReferenciaId { get; set; }

        public int? UsuarioId { get; set; }
        public Usuario? Usuario { get; set; }

        public string? Notas { get; set; }
        public DateTime FechaMovimiento { get; set; } = DateTime.UtcNow;
    }
}
