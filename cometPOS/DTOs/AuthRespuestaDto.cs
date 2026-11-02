namespace cometPOS.DTOs
{
    public class AuthRespuestaDto
    {
        public string Token { get; set; } = string.Empty;
        public string Nombre { get; set; } = string.Empty;
        public string Rol { get; set; } = string.Empty;
        public int? SucursalId { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
    }
}
