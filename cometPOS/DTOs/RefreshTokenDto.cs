using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class RefreshTokenDto
    {
        [Required]
        public string RefreshToken { get; set; } = string.Empty;
    }
}
