using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class CambiarPasswordDto
    {
        [Required]
        public string PasswordActual { get; set; } = string.Empty;
        [Required, MinLength(8)]
        public string PasswordNueva { get; set; } = string.Empty;
    }
}
