namespace SonicSpectrum.Domain.Entities
{
    public class Genre
    {
        public string GenreId { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; } = null!;
        public string? GenreImage { get; set; }
        public virtual ICollection<Track>? Tracks { get; set; }
    }
}
