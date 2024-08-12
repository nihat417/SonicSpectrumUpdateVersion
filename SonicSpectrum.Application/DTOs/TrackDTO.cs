using Microsoft.AspNetCore.Http;

namespace SonicSpectrum.Application.DTOs
{
    public class TrackDTO
    {
        public string? Title { get; set; }
        public IFormFile? FilePath { get; set; }
        public IFormFile? ImagePath { get; set; }
        public string? ArtistId { get; set; }
        public string? GenreId {  get; set; }
        public string? AlbumId {  get; set; }
        public IEnumerable<string>? AlbumTitles { get; set; }
        public IEnumerable<string>? GenreNames { get; set; }
        public IEnumerable<LyricDTO>? Lyrics { get; set; }
    }
}
