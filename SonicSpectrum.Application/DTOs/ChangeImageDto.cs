using Microsoft.AspNetCore.Http;

namespace SonicSpectrum.Application.DTOs
{
    public class ChangeImageDto
    {
        public string Email { get; set; } = null!;
        public IFormFile NewImage { get; set; } = null!;
    }
}
