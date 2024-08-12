namespace SonicSpectrum.Domain.Entities
{
    public class Artist
    {
        private string _name = "unknown";
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get => _name; set => _name = string.IsNullOrWhiteSpace(value) ? "unknown": value; }
        public string? ArtistImage {  get; set; } 
        public virtual ICollection<Track> ?Tracks { get; set; }
        public virtual ICollection<Album> ?Albums { get; set; }
    }
}
