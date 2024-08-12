using Microsoft.AspNetCore.Http;

namespace SonicSpectrum.Application.DTOs
{
    public class PlaylistDTO
    {
        public string UserId { get; set; } = null!;
        public string PlaylistName { get; set; } = null!;
        public IFormFile? PlaylistImage { get; set; }
    }
}
