namespace SonicSpectrum.Application.DTOs
{
    public class UpdateUserStatisticsDto
    {
        public string UserId { get; set; }
        public string TrackId { get; set; }
        public int MinutesListened { get; set; }
    }
}
