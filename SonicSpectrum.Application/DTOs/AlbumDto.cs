using Microsoft.AspNetCore.Http;

namespace SonicSpectrum.Application.DTOs
{
    public class AlbumDto
    {
        public string Title { get; set; } = null!;
        public IFormFile? AlbumImage { get; set; }
        public string ArtistId { get; set; } = null!;
    }
}
