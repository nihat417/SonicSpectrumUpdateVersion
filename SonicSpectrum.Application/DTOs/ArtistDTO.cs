using Microsoft.AspNetCore.Http;

namespace SonicSpectrum.Application.DTOs
{
    public class ArtistDTO
    {
        public string? Name { get; set; }
        public IFormFile? ArtistImage { get; set; }
    }
}
