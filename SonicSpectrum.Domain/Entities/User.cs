using Microsoft.AspNetCore.Identity;

namespace SonicSpectrum.Domain.Entities
{
    public class User:IdentityUser
    {
        public string FullName { get; set; } = null!;
        public int Age { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime? CreatedTime { get; set; }
        public bool IsProfileOpen { get; set; }
        public virtual ICollection<Playlist>? Playlists { get; set; }
        public virtual ICollection<UserListeningStatistics>? ListeningStatistics { get; set; }

        public virtual ICollection<Follow> Followings { get; set; } = new List<Follow>();
        public virtual ICollection<Follow> Followers  { get; set; } = new List<Follow>();

        public virtual ICollection<Genre>? FavoriteGenres { get; set; } = new List<Genre>();
    }
}
