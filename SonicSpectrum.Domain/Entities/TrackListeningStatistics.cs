namespace SonicSpectrum.Domain.Entities
{
    public class TrackListeningStatistics
    {
        public string TrackListeningStatisticsId { get; set; } = Guid.NewGuid().ToString();
        public string TrackId { get; set; } = null!;
        public int TimesListened { get; set; }
        public int TotalListeningMinutes { get; set; }
        public virtual Track Track { get; set; } = null!;
    }
}
