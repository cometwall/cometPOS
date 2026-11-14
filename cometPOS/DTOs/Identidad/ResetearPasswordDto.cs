using System.ComponentModel.DataAnnotations;

namespace cometPOS.DTOs
{
    public class ResetearPasswordDto
    {
        [Required, MinLength(8)]
        public string PasswordNueva { get; set; } = string.Empty;
    }
}
