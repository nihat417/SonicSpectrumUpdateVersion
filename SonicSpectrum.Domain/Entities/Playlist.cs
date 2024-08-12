namespace SonicSpectrum.Domain.Entities
{
    public class Playlist
    {
        public string PlaylistId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string UserId { get; set; } = null!;
        public string? PlaylistImage {  get; set; } 
        public virtual User? User { get; set; }
        public virtual ICollection<Track>? Tracks { get; set; }
    }
}
