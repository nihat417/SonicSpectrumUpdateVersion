using System.ComponentModel.DataAnnotations;

namespace SonicSpectrum.Application.DTOs
{
    public class ForgotDTO
    {
        [Required]
        public string Email { get; set; } = null!;
    }
}
