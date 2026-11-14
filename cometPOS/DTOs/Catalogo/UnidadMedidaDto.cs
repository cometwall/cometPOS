using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs.Catalogo
{
    public class UnidadMedidaDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Abreviatura { get; set; } = string.Empty;
        public string? ClaveSat { get; set; }
        public bool PermiteDecimales { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
