namespace SonicSpectrum.Domain.Entities
{
    public class Album
    {
        public string AlbumId { get; set; } = Guid.NewGuid().ToString();
        public string? Title { get; set; }
        public string? AlbumImage { get; set; }
        public virtual ICollection<Track>? Tracks { get; set; }
        public string? ArtistId { get; set; }
        public virtual Artist? Artist { get; set; }
    }
}
