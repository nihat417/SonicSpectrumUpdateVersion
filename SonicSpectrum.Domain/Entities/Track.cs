namespace SonicSpectrum.Domain.Entities
{
    public class Track
    {
        public string TrackId { get; set; } = Guid.NewGuid().ToString();
        public string Title { get; set; } = null!;
        public string FilePath { get; set; } = null!;
        public string? ImagePath { get; set; }
        public string? ArtistId { get; set; }
        public string? AlbumId { get; set; }
        public virtual Artist? Artist { get; set; }
        public virtual Album? Album { get; set; }
        public virtual ICollection<Lyric>? Lyrics { get; set; }
        public virtual ICollection<Album>? Albums { get; set; }
        public virtual ICollection<Genre>? Genres { get; set; }
        public virtual ICollection<Playlist>? Playlists { get; set; }
        public virtual ICollection<TrackListeningStatistics>? ListeningStatistics { get; set; }
        public virtual ICollection<UserListeningStatistics>? UserListeningStatistics { get; set; }
    }
}


