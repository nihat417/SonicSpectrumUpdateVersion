namespace SonicSpectrum.Domain.Entities
{
    public class Lyric
    {
        public string LyricId { get; set; } =Guid.NewGuid().ToString();
        public string? Text { get; set; }
        public string? TrackId { get; set; }
        public virtual Track? Track { get; set; }
    }
}
