using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class UnidadMedida
    {
        public int Id { get; set; }
        public int EmpresaId { get; set; }
        public Empresa Empresa { get; set; } = null!;

        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Abreviatura { get; set; } = string.Empty;
        public string? ClaveSat { get; set; }
        public bool PermiteDecimales { get; set; } = false;
        public bool Activa { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;
    }
}
