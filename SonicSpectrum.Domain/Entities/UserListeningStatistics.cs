namespace SonicSpectrum.Domain.Entities
{
    public class UserListeningStatistics
    {
        public string UserListeningStatisticsId { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; } = null!;
        public string TrackId { get; set; } = null!;
        public int TimesListened { get; set; }
        public int TotalListeningMinutes { get; set; }
        public virtual User User { get; set; } = null!;
        public virtual Track Track { get; set; } = null!;
    }
}
