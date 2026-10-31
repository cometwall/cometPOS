namespace cometPOS.DTOs
{
    public class SucursalDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Direccion { get; set; } = string.Empty;
        public string? Telefono { get; set; }
        public bool Activa { get; set; }
        public DateTime FechaCreacion { get; set; }
    }
}
