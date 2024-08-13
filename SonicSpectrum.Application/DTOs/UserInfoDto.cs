namespace SonicSpectrum.Application.DTOs
{
    public class UserInfoDto
    {
        public string? Id { get; set; }
        public string? UserName { get; set; }
        public string? FullName { get; set; }
        public int Age { get; set; }
        public string? ImageUrl { get; set; }
        public bool IsProfileOpen { get; set; }
        public ICollection<PlaylistDTO>? Playlists { get; set; }

    }
}