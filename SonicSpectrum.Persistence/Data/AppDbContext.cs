using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SonicSpectrum.Domain.Entities;

namespace SonicSpectrum.Persistence.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : IdentityDbContext<User>(options)
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseLazyLoadingProxies();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Artist>()
                .HasMany(a => a.Albums)
                .WithOne(album => album.Artist)
                .HasForeignKey(album => album.ArtistId);

            modelBuilder.Entity<Album>()
                .HasMany(album => album.Tracks)
                .WithOne()
                .HasForeignKey(track => track.AlbumId);

            modelBuilder.Entity<Album>()
                .HasMany(album => album.Tracks)
                .WithOne(track => track.Album)
                .HasForeignKey(track => track.AlbumId);


            modelBuilder.Entity<Track>()
                .HasMany(track => track.Genres)
                .WithMany(genre => genre.Tracks)
                .UsingEntity(j => j.ToTable("TrackGenres"));

            modelBuilder.Entity<Track>()
                .HasMany(track => track.Lyrics)
                .WithOne(lyric => lyric.Track)
                .HasForeignKey(lyric => lyric.TrackId);

            modelBuilder.Entity<Track>()
                .HasMany(track => track.Playlists)
                .WithMany(playlist => playlist.Tracks)
                .UsingEntity(j => j.ToTable("TrackPlaylists"));

            modelBuilder.Entity<TrackListeningStatistics>()
                .HasOne(tls => tls.Track)
                .WithMany(t => t.ListeningStatistics)
                .HasForeignKey(tls => tls.TrackId);

            modelBuilder.Entity<UserListeningStatistics>()
                .HasOne(uls => uls.User)
                .WithMany(u => u.ListeningStatistics)
                .HasForeignKey(uls => uls.UserId);

            modelBuilder.Entity<UserListeningStatistics>()
                .HasOne(uls => uls.Track)
                .WithMany(t => t.UserListeningStatistics)
                .HasForeignKey(uls => uls.TrackId);


            modelBuilder.Entity<Follow>()
                 .HasKey(f => new { f.FollowerId, f.FolloweeId });

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Follower)
                .WithMany(u => u.Followings)
                .HasForeignKey(f => f.FollowerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Follow>()
                .HasOne(f => f.Followee)
                .WithMany(u => u.Followers)
                .HasForeignKey(f => f.FolloweeId)
                .OnDelete(DeleteBehavior.Restrict);


            modelBuilder.Entity<Message>()
                .HasOne(m => m.Sender)
                .WithMany()
                .HasForeignKey(m => m.SenderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Message>()
                .HasOne(m => m.Receiver)
                .WithMany()
                .HasForeignKey(m => m.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
        }



        public DbSet<Artist> Artists { get; set; }
        public DbSet<Album> Albums { get; set; }
        public DbSet<Track> Tracks { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Lyric> Lyrics { get; set; }
        public DbSet<Message> Messages { get; set; }
        public DbSet<Playlist> Playlists { get; set; }
        public DbSet<TrackListeningStatistics> TrackListeningStatistics { get; set; }
        public DbSet<UserListeningStatistics> UserListeningStatistics { get; set; }
        public DbSet<Follow> Follows { get; set; }
    }
}
