using Microsoft.AspNetCore.Http;

namespace SonicSpectrum.Application.DTOs
{
    public class GenreDTO
    {
        public string? Name { get; set; }
        public IFormFile? GenreImage { get; set; }
    }
}
