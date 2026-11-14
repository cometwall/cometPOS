using System.ComponentModel.DataAnnotations;

namespace cometPOS.Models
{
    public class Empresa
    {
        public int Id { get; set; }
        [Required]
        public string Nombre { get; set; } = string.Empty;
        [Required]
        public string Identificador { get; set; } = string.Empty; // RFC, NIT, etc.
        public bool Activa { get; set; } = true;
        public DateTime FechaCreacion { get; set; } = DateTime.UtcNow;

        public ICollection<Sucursal> Sucursales { get; set; } = new List<Sucursal>();
        public ICollection<Usuario> Usuarios { get; set; } = new List<Usuario>();
    }
}
