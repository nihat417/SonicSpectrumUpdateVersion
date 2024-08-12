using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Models;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Application.Services;
using SonicSpectrum.Domain.Entities;
using SonicSpectrum.Persistence.Data;

namespace SonicSpectrum.Application.Repository.Concrete
{
    public class MusicSettingService(AppDbContext _context) : IMusicSettingService
    {
        #region get

        public async Task<object> GetTrackById(string musicId)
        {
            if (string.IsNullOrEmpty(musicId)) throw new ArgumentNullException(nameof(musicId), "Music ID cannot be null or empty.");

            try
            {
                var music = await _context.Tracks.AsNoTracking()
                                                 .Where(m => m.TrackId == musicId)
                                                 .Select(t=> new { 
                                                     t.TrackId,
                                                     t.Title,
                                                     t.AlbumId,
                                                     AlbumTitle = t.Album!.Title,
                                                     t.ArtistId,
                                                     t.Artist!.Name,
                                                     t.FilePath,
                                                     t.ImagePath,
                                                 })
                                                 .FirstOrDefaultAsync();

                if (music == null) throw new KeyNotFoundException($"Music with ID '{musicId}' not found.");

                return music;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while retrieving music with ID '{musicId}': {ex.Message}");
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetAllTracksAsync(int pageNumber, int pageSize)
        {
            var offset = (pageNumber - 1) * pageSize;
            var query = @"
                SELECT t.TrackId, t.Title, t.FilePath, t.ImagePath, a.Name AS ArtistName, 
                       t.ArtistId, t.AlbumId, al.Title AS AlbumTitle
                FROM Tracks t
                LEFT JOIN Artists a ON t.ArtistId = a.Id
                LEFT JOIN Albums al ON t.AlbumId = al.AlbumId
                ORDER BY t.TrackId
                OFFSET @Offset ROWS FETCH NEXT @PageSize ROWS ONLY";

            var parameters = new[]
            {
                new SqlParameter("@Offset", offset),
                new SqlParameter("@PageSize", pageSize)
            };

            var tracks = await _context.Tracks
                .FromSqlRaw(query, parameters)
                .Select(t => new
                {
                    t.TrackId,
                    t.Title,
                    t.FilePath,
                    t.ImagePath,
                    ArtistName = t.Artist!.Name,
                    t.ArtistId,
                    t.AlbumId,
                    AlbumTitle = t.Album!.Title
                })
                .ToListAsync();

            return tracks;
        }

        public async Task<IEnumerable<object>> GetRandomTracks()
        {
            var random = new Random();
            var tracks = await _context.Tracks
                .Select(t => new
                {
                    t.TrackId,
                    t.Title,
                    t.AlbumId,
                    AlbumTitle = t.Album!.Title,
                    t.ArtistId,
                    ArtistName = t.Artist!.Name,
                    t.FilePath,
                    t.ImagePath,
                })
                .ToListAsync();

            var randomTracks = tracks.OrderBy(x => random.Next()).Take(5).ToList();
            return randomTracks;
        }

        public async Task<IEnumerable<object>> GetAllAlbumsForArtistAsync(string artistId, int pageNumber, int pageSize)
        {
            var artist = await _context.Artists.FindAsync(artistId);
            if (artist == null) return Enumerable.Empty<object>();

            var albums = await _context.Albums
                                        .AsNoTracking()
                                        .Where(album => album.ArtistId == artistId)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .Select(album => new
                                        {
                                            album.AlbumId,
                                            album.Title,
                                            album.ArtistId
                                        })
                                        .ToListAsync();
            return albums;
        }

        public async Task<IEnumerable<object>> GetMusicFromAlbum(string albumId, int pageNumber, int pageSize)
        {
            var album = await _context.Albums.FindAsync(albumId);
            if(album == null) return Enumerable.Empty<object>();

            var tracks = await _context.Tracks
                                        .AsNoTracking()
                                        .Where(track => track.AlbumId == albumId)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .Select(track => new
                                        {
                                            track.TrackId,
                                            track.Title,
                                            track.FilePath,
                                            track.ImagePath,
                                            track.ArtistId,
                                            track.AlbumId
                                        })
                                .ToListAsync();
            return tracks;
        }

        public async Task<IEnumerable<object>> GetAllArtistsAsync(int pageNumber, int pageSize)
        {
            try
            {
                var artists = await _context.Artists
                    .AsNoTracking()
                    .OrderBy(a => a.Name) 
                    .Skip((pageNumber - 1) * pageSize) 
                    .Take(pageSize) 
                    .Select(a => new 
                    {
                        a.Id,
                        a.Name
                    })
                    .ToListAsync();

                return artists;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error occurred while retrieving artists: {ex.Message}");
            }
        }

        public async Task<IEnumerable<object>> GetMusicFromPlaylist(string playlistId, int pageNumber, int pageSize)
        {
            var playlist = await _context.Playlists.FindAsync(playlistId);
            if (playlist == null) return Enumerable.Empty<object>();

            var trackIds = playlist.Tracks!.Select(pTrack => pTrack.TrackId).ToList();

            var tracks = await _context.Tracks
                                        .AsNoTracking()
                                        .Where(track => trackIds.Contains(track.TrackId))
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .Select(track => new
                                        {
                                            track.TrackId,
                                            track.Title,
                                            track.FilePath,
                                            track.ImagePath,
                                            track.ArtistId,
                                            track.AlbumId
                                        })
                                        .ToListAsync();
            return tracks;
        }

        public async Task<IEnumerable<object>> GetPlaylistFromUser(string userId, int pageNumber, int pageSize)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null) return Enumerable.Empty<object>();

            var playlists = await _context.Playlists
                                        .AsNoTracking()
                                        .Where(playlist => playlist.UserId == userId)
                                        .OrderBy(playlist => playlist.Name)
                                        .Skip((pageNumber - 1) * pageSize)
                                        .Take(pageSize)
                                        .Select(playlist => new
                                        {
                                            playlist.PlaylistId,
                                            playlist.Name,
                                            playlist.PlaylistImage
                                        })
                                .ToListAsync();
            return playlists;
        }

        public async Task<IEnumerable<object>> GetRecommendedTracksAsync(string userId)
        {
            var user = await _context.Users
                .Include(u => u.FavoriteGenres)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
                throw new ArgumentException($"User with ID '{userId}' not found.");

            var random = new Random();
            /*List<object> recommendedTracks;*/

            if (user.FavoriteGenres == null || !user.FavoriteGenres.Any())
            {
                var query = @"
                    SELECT TOP 10 
                        t.TrackId, 
                        t.Title, 
                        t.FilePath, 
                        t.ImagePath, 
                        a.Name AS ArtistName, 
                        t.ArtistId, 
                        t.AlbumId, 
                        al.Title AS AlbumTitle
                    FROM Tracks t
                    LEFT JOIN Artists a ON t.ArtistId = a.Id
                    LEFT JOIN Albums al ON t.AlbumId = al.AlbumId
                    ORDER BY NEWID()";

                var recommendedTracks = await _context.Tracks
                    .FromSqlRaw(query)
                    .Select(t => new
                    {
                        t.TrackId,
                        t.Title,
                        t.FilePath,
                        t.ImagePath,
                        ArtistName = t.Artist!.Name,
                        t.ArtistId,
                        t.AlbumId,
                        AlbumTitle = t.Album!.Title
                    })
                    .ToListAsync();
                return recommendedTracks;
            }
            else
            {
                var favoriteGenreIds = user.FavoriteGenres.Select(g => g.GenreId).ToList();
                var genreIdParameters = string.Join(", ", favoriteGenreIds.Select((_, index) => $"@p{index}"));

                var query = $@"
                    SELECT TOP 10 
                        t.TrackId, 
                        t.Title, 
                        t.FilePath, 
                        t.ImagePath, 
                        a.Name AS ArtistName, 
                        t.ArtistId, 
                        t.AlbumId, 
                        al.Title AS AlbumTitle
                    FROM Tracks t
                    LEFT JOIN Artists a ON t.ArtistId = a.Id
                    LEFT JOIN Albums al ON t.AlbumId = al.AlbumId
                    LEFT JOIN TrackGenres tg ON t.TrackId = tg.TrackId
                    WHERE tg.GenreId IN ({genreIdParameters})
                    ORDER BY NEWID()";

                var parameters = favoriteGenreIds.Select((genreId, index) => new SqlParameter($"@p{index}", genreId)).ToArray();

                var recommendedTracks = await _context.Tracks
                    .FromSqlRaw(query, parameters)
                    .Select(t => new
                    {
                        t.TrackId,
                        t.Title,
                        t.FilePath,
                        t.ImagePath,
                        ArtistName = t.Artist!.Name,
                        t.ArtistId,
                        t.AlbumId,
                        AlbumTitle = t.Album!.Title
                    })
                    .ToListAsync();
                return recommendedTracks;
            }
        }

        public async Task<IEnumerable<object>> GetPopularArtistsAsync()
        {
            var popularArtists = await _context.Artists.AsNoTracking()
                .Select(a => new
                {
                    a.Id,
                    a.Name,
                    a.ArtistImage,
                    PopularityScore = a.Tracks!.Sum(t => t.ListeningStatistics!.Sum(ls => ls.TimesListened))
                })
                .OrderByDescending(a => a.PopularityScore)
                .Take(10)
                .ToListAsync();

            return popularArtists;
        }

        public async Task<IEnumerable<object>> GetRecommendedAlbumsAsync()
        {
            var random = new Random();
            var albums = await _context.Albums.AsNoTracking()
                .Select(a => new
                {
                    a.AlbumId,
                    a.Title,
                    a.AlbumImage,
                    a.ArtistId,
                    ArtistName = a.Artist!.Name
                })
                .ToListAsync();

            var recommendedAlbums = albums.OrderBy(x => random.Next()).Take(10).ToList();
            return recommendedAlbums;
        }


        public async Task<object> SearchAsync(string query, int pageNumber, int pageSize)
        {
            if (string.IsNullOrWhiteSpace(query))
                throw new ArgumentNullException(nameof(query), "Search query cannot be null or empty.");

            try
            {
                var offset = (pageNumber - 1) * pageSize;

                // Поиск по музыке
                var tracksQuery = _context.Tracks
                    .AsNoTracking()
                    .Where(t => t.Title.Contains(query) || t.Artist.Name.Contains(query) || t.Album.Title.Contains(query))
                    .Select(t => new
                    {
                        Type = "Track",
                        t.TrackId,
                        t.Title,
                        t.FilePath,
                        t.ImagePath,
                        ArtistName = t.Artist.Name,
                        AlbumTitle = t.Album.Title
                    });

                // Поиск по исполнителям
                var artistsQuery = _context.Artists
                    .AsNoTracking()
                    .Where(a => a.Name.Contains(query))
                    .Select(a => new
                    {
                        Type = "Artist",
                        a.Id,
                        a.Name
                    });

                // Поиск по альбомам
                var albumsQuery = _context.Albums
                    .AsNoTracking()
                    .Where(a => a.Title.Contains(query) || a.Artist.Name.Contains(query))
                    .Select(a => new
                    {
                        Type = "Album",
                        a.AlbumId,
                        a.Title,
                        a.ArtistId,
                        ArtistName = a.Artist.Name
                    });

                // Поиск по плейлистам
                var playlistsQuery = _context.Playlists
                    .AsNoTracking()
                    .Where(p => p.Name.Contains(query))
                    .Select(p => new
                    {
                        Type = "Playlist",
                        p.PlaylistId,
                        p.Name
                    });

                // Выполнение запросов и объединение результатов
                var tracks = await tracksQuery
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var artists = await artistsQuery
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var albums = await albumsQuery
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var playlists = await playlistsQuery
                    .Skip(offset)
                    .Take(pageSize)
                    .ToListAsync();

                var results = new
                {
                    Tracks = tracks,
                    Artists = artists,
                    Albums = albums,
                    Playlists = playlists
                };

                return results;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while searching: {ex.Message}");
                throw;
            }
        }

        #endregion

        #region post

        public async Task<OperationResult> AddAlbumAsync(AlbumDto albumDto)
        {
            var result = new OperationResult();

            var artist = await _context.Artists.FirstOrDefaultAsync(art => art.Id == albumDto.ArtistId);

            if (artist == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Artist with ID {albumDto.ArtistId} is not found.";
                return result;
            }

            if(albumDto == null || albumDto.Title == null || string.IsNullOrEmpty(albumDto.Title))
            {
                result.Success = false;
                result.ErrorMessage = $"Album is null or empty";
                return result;
            }

            try
            {
                var existingAlbum = await _context.Albums.FirstOrDefaultAsync(a => a.ArtistId == artist.Id && a.Title == albumDto.Title);
                if (existingAlbum != null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Album with the same title already exists for this artist.";
                    return result;
                }

                var album = new Album { Title = albumDto.Title,ArtistId = artist.Id, };

                album.AlbumImage = (albumDto.AlbumImage != null) ? await UploadFileHelper.UploadFile(albumDto.AlbumImage!, "albumphoto", album.AlbumId) :
                    "https://seventysoundst.blob.core.windows.net/albumphoto/defalbumphoto.jpg";

                await _context.Albums.AddAsync(album);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"{album} Added successfully";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> AddArtistAsync(ArtistDTO artistDto)
        {
            var result = new OperationResult();

            if (artistDto == null || artistDto.Name == null)
            {
                result.Success = false;
                result.ErrorMessage = "ArtistDTO or artist name is null.";
                return result;
            }

            try
            {
                var artist = new Artist { 
                    Name = artistDto.Name,
                };

                artist.ArtistImage = (artistDto.ArtistImage != null) ?
                    await UploadFileHelper.UploadFile(artistDto.ArtistImage!, "artistphoto", artist.Id) :
                    "https://seventysoundst.blob.core.windows.net/artistphoto/defartistphoto.jpg";

                await _context.Artists.AddAsync(artist);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"{artist} Added successfully";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> AddGenreAsync(GenreDTO genreDto)
        {
            var result = new OperationResult();

            if (genreDto == null || genreDto.Name == null || string.IsNullOrEmpty(genreDto.Name))
            {
                result.Success = false;
                result.ErrorMessage = "GenreDTO or genre name is null.";
                return result;
            }

            try
            {
                var existingGenre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreDto.Name);
                if (existingGenre == null)
                {
                    var genre = new Genre {
                        Name = genreDto.Name,};

                    genre.GenreImage = (genreDto.GenreImage != null)
                        ? await UploadFileHelper.UploadFile(genreDto.GenreImage!, "genrephoto", genre.GenreId)
                        : "https://seventysoundst.blob.core.windows.net/albumphoto/defalbumphoto.jpg";

                    await _context.Genres.AddAsync(genre);
                    await _context.SaveChangesAsync();
                    result.Success = true;
                    result.Message = $"{genre} Added successfully"; 
                    return result;
                }

                result.Success = false;
                result.ErrorMessage = $"{genreDto.Name} already exists.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> AddTrackAsync(TrackDTO trackDto)
        {
            var result = new OperationResult();

            if (trackDto == null || trackDto.Title == null || trackDto.ArtistId == null || trackDto.AlbumId == null
                || trackDto.FilePath == null || trackDto.ImagePath == null || string.IsNullOrEmpty(trackDto.Title))
            {
                result.Success = false;
                result.ErrorMessage = "One or more required fields are null.";
                return result;
            }

            try
            {
                var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == trackDto.ArtistId);
                if (artist == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Artist with ID {trackDto.ArtistId} is not found";
                    return result;
                }

                var album = await _context.Albums.FirstOrDefaultAsync(a => a.AlbumId == trackDto.AlbumId && a.ArtistId == artist.Id);
                if (album == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Album with ID {trackDto.AlbumId} does not belong to the specified artist";
                    return result;
                }

                var track = new Track
                {
                    Title = trackDto.Title,
                    ArtistId = trackDto.ArtistId,
                    AlbumId = trackDto.AlbumId,
                };

                track.FilePath = await UploadFileHelper.UploadFile(trackDto.FilePath, "musicplay", track.TrackId);
                track.ImagePath = await UploadFileHelper.UploadFile(trackDto.ImagePath, "musicphoto", track.TrackId);

                track.Albums = new HashSet<Album>();
                track.Genres = new HashSet<Genre>();
                track.Lyrics = new HashSet<Lyric>();

                if (trackDto.AlbumTitles != null)
                {
                    foreach (var albumTitle in trackDto.AlbumTitles)
                    {
                        var albumFromTitles = await _context.Albums.FirstOrDefaultAsync(a => a.Title == albumTitle && a.ArtistId == artist.Id);
                        if (albumFromTitles != null)
                            track.Albums.Add(albumFromTitles);
                    }
                }

                if (trackDto.GenreNames != null)
                {
                    foreach (var genreName in trackDto.GenreNames)
                    {
                        var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                        if (genre != null)
                            track.Genres.Add(genre);
                    }
                }

                if (trackDto.Lyrics != null)
                {
                    foreach (var lyricDto in trackDto.Lyrics)
                    {
                        string lyricText = lyricDto.Text!;
                        var lyric = new Lyric { Text = lyricText };
                        track.Lyrics.Add(lyric);
                    }
                }

                await _context.Tracks.AddAsync(track);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"{track} Added successfully";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
            }

            return result;
        }

        public async Task<OperationResult> AddGenreToTrackAsync(string trackId, string genreName)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(trackId) || string.IsNullOrEmpty(genreName))
            {
                result.Success = false;
                result.ErrorMessage = "Track ID or genre name is null or empty.";
                return result;
            }

            try
            {
                var track = await _context.Tracks.FirstOrDefaultAsync(t => t.TrackId == trackId);
                if (track == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Track with ID '{trackId}' not found.";
                    return result;
                }

                var genre = await _context.Genres.FirstOrDefaultAsync(g => g.Name == genreName);
                if (genre == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Genre with name '{genreName}' not found.";
                    return result;
                }

                track.Genres!.Add(genre);

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"Genre '{genre.Name}' added to track '{track.Title}' successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> AddLyricsToTrackAsync(string trackId, string lyricsText)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(trackId) || string.IsNullOrEmpty(lyricsText))
            {
                result.Success = false;
                result.ErrorMessage = "Track ID or lyrics text is null or empty.";
                return result;
            }

            try
            {
                var track = await _context.Tracks.FirstOrDefaultAsync(t => t.TrackId == trackId);
                if (track == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Track with ID '{trackId}' not found.";
                    return result;
                }

                var lyric = new Lyric { Text = lyricsText };

                track.Lyrics!.Add(lyric);

                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"Lyrics added to track '{track.Title}' successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> CreatePlaylistAsync(PlaylistDTO requestDto)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(requestDto.UserId) || string.IsNullOrEmpty(requestDto.PlaylistName))
            {
                result.Success = false;
                result.ErrorMessage = "User ID or playlist name is null or empty.";
                return result;
            }

            try
            {
                var existingPlaylist = await _context.Playlists.FirstOrDefaultAsync(p => p.UserId == requestDto.UserId && p.Name == requestDto.PlaylistName);
                if (existingPlaylist != null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Playlist with the same name already exists for this user.";
                    return result;
                }

                var playlist = new Playlist
                {
                    Name = requestDto.PlaylistName,
                    UserId = requestDto.UserId,
                };

                playlist.PlaylistImage = (requestDto.PlaylistImage != null) ?
                    await UploadFileHelper.UploadFile(requestDto.PlaylistImage!, "playlistphoto", playlist.PlaylistId) :
                    "https://musicstrgac.blob.core.windows.net/playlistphoto/defplaylist.jpg";

                _context.Playlists.Add(playlist);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Playlist created successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> AddTrackToPlaylistAsync(TrackPlaylistDTO requestDto)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(requestDto.PlaylistId) || string.IsNullOrEmpty(requestDto.TrackId))
            {
                result.Success = false;
                result.ErrorMessage = "Playlist ID or track ID is null or empty.";
                return result;
            }

            try
            {
                var playlist = await _context.Playlists.Include(p => p.Tracks).FirstOrDefaultAsync(p => p.PlaylistId == requestDto.PlaylistId);
                var track = await _context.Tracks.FindAsync(requestDto.TrackId);

                if (playlist == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Playlist with ID '{requestDto.PlaylistId}' not found.";
                    return result;
                }

                if (track == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Track with ID '{requestDto.TrackId}' not found.";
                    return result;
                }

                if (playlist.Tracks!.Any(t => t.TrackId == requestDto.TrackId))
                {
                    result.Success = false;
                    result.ErrorMessage = "Track already exists in the playlist.";
                    return result;
                }

                playlist.Tracks!.Add(track);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Track added to playlist successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task UpdateTrackListeningStatisticsAsync(string trackId, int minutesListened)
        {
            var trackStatistics = await _context.TrackListeningStatistics.FirstOrDefaultAsync(ts => ts.TrackId == trackId);

            if (trackStatistics == null)
            {
                trackStatistics = new TrackListeningStatistics
                {
                    TrackId = trackId,
                    TimesListened = 1,
                    TotalListeningMinutes = minutesListened
                };

                await _context.TrackListeningStatistics.AddAsync(trackStatistics);
            }
            else
            {
                trackStatistics.TimesListened++;
                trackStatistics.TotalListeningMinutes += minutesListened;
            }

            await _context.SaveChangesAsync();
        }

        public async Task UpdateUserListeningStatisticsAsync(string userId, string trackId, int minutesListened)
        {
            var userStatistics = await _context.UserListeningStatistics.FirstOrDefaultAsync(us => us.UserId == userId && us.TrackId == trackId);

            if (userStatistics == null)
            {
                userStatistics = new UserListeningStatistics
                {
                    UserId = userId,
                    TrackId = trackId,
                    TimesListened = 1,
                    TotalListeningMinutes = minutesListened
                };

                await _context.UserListeningStatistics.AddAsync(userStatistics);
            }
            else
            {
                userStatistics.TimesListened++;
                userStatistics.TotalListeningMinutes += minutesListened;
            }

            await _context.SaveChangesAsync();
        }


        #endregion

        #region Edit

        public async Task<OperationResult> EditAlbumAsync(string albumId, AlbumDto albumDto)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(albumId))
            {
                result.Success = false;
                result.ErrorMessage = "Album ID is null or empty.";
                return result;
            }

            var existingAlbum = await _context.Albums.FindAsync(albumId);
            if (existingAlbum == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Album with ID {albumId} not found.";
                return result;
            }

            if (albumDto == null || string.IsNullOrEmpty(albumDto.Title))
            {
                result.Success = false;
                result.ErrorMessage = "AlbumDTO or album title is null or empty.";
                return result;
            }

            existingAlbum.Title = albumDto.Title;

            try
            {
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"Album with ID {albumId} updated successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> EditArtistAsync(string artistId, ArtistDTO artistDto)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(artistId))
            {
                result.Success = false;
                result.ErrorMessage = "Artist ID is null or empty.";
                return result;
            }

            var existingArtist = await _context.Artists.FindAsync(artistId);
            if (existingArtist == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Artist with ID {artistId} not found.";
                return result;
            }

            if (artistDto == null || string.IsNullOrEmpty(artistDto.Name))
            {
                result.Success = false;
                result.ErrorMessage = "ArtistDTO or artist name is null or empty.";
                return result;
            }

            existingArtist.Name = artistDto.Name; 

            try
            {
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"Artist with ID {artistId} updated successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> EditGenreAsync(string genreId, GenreDTO genreDto)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(genreId))
            {
                result.Success = false;
                result.ErrorMessage = "Genre ID is null or empty.";
                return result;
            }

            var existingGenre = await _context.Genres.FindAsync(genreId);
            if (existingGenre == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Genre with ID {genreId} not found.";
                return result;
            }

            if (genreDto == null || string.IsNullOrEmpty(genreDto.Name))
            {
                result.Success = false;
                result.ErrorMessage = "GenreDTO or genre name is null or empty.";
                return result;
            }

            existingGenre.Name = genreDto.Name; 

            try
            {
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"Genre with ID {genreId} updated successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> EditTrackAsync(string trackId, TrackDTO trackDto)
        {
            var result = new OperationResult();

            if (string.IsNullOrEmpty(trackId))
            {
                result.Success = false;
                result.ErrorMessage = "Track ID is null or empty.";
                return result;
            }

            var existingTrack = await _context.Tracks.FindAsync(trackId);
            if (existingTrack == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Track with ID {trackId} not found.";
                return result;
            }

            if (trackDto == null || string.IsNullOrEmpty(trackDto.Title) || string.IsNullOrEmpty(trackDto.ArtistId)
                || string.IsNullOrEmpty(trackDto.AlbumId) || trackDto.FilePath == null
                || trackDto.ImagePath == null)
            {
                result.Success = false;
                result.ErrorMessage = "One or more required fields are null or empty.";
                return result;
            }

            var artist = await _context.Artists.FirstOrDefaultAsync(a => a.Id == trackDto.ArtistId);
            if (artist == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Artist with ID {trackDto.ArtistId} is not found";
                return result;
            }

            var album = await _context.Albums.FirstOrDefaultAsync(a => a.AlbumId == trackDto.AlbumId && a.ArtistId == artist.Id);
            if (album == null)
            {
                result.Success = false;
                result.ErrorMessage = $"Album with ID {trackDto.AlbumId} does not belong to the specified artist";
                return result;
            }

            existingTrack.Title = trackDto.Title ?? existingTrack.Title;
            existingTrack.ArtistId = trackDto.ArtistId ?? existingTrack.ArtistId;
            existingTrack.AlbumId = trackDto.AlbumId ?? existingTrack.AlbumId;

            try
            {
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = $"Track with ID {trackId} updated successfully.";
                return result;
            }
            catch (DbUpdateException ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Database update error: {ex.Message}";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        #endregion

        #region Delete

        public async Task<OperationResult> DeleteArtistAsync(string artistId)
        {
            var result = new OperationResult();

            var artist = await _context.Artists.FindAsync(artistId);
            if (artist != null)
            {
                try
                {
                    _context.Artists.Remove(artist);
                    await _context.SaveChangesAsync();

                    await UploadFileHelper.DeleteFile(artistId!, "artistphoto");

                    result.Success = true;
                    result.Message = "Artist deleted successfully.";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"{artist} is null";
            }

            return result;
        }

        public async Task<OperationResult> DeleteAlbumAsync(string albumId)
        {
            var result = new OperationResult();

            var album = await _context.Albums.FindAsync(albumId);
            if (album != null)
            {
                try
                {
                    _context.Albums.Remove(album);
                    await _context.SaveChangesAsync();

                    await UploadFileHelper.DeleteFile(albumId!, "albumphoto");

                    result.Success = true;
                    result.Message = "Album deleted successfully.";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"Album with ID {albumId} is not found";
            }

            return result;
        }

        public async Task<OperationResult> DeleteGenreAsync(string genreId)
        {
            var result = new OperationResult();

            var genre = await _context.Genres.FindAsync(genreId);
            if (genre != null)
            {
                try
                {
                    _context.Genres.Remove(genre);
                    await _context.SaveChangesAsync();

                    await UploadFileHelper.DeleteFile(genreId!, "genrephoto");

                    result.Success = true;
                    result.Message = "Genre deleted successfully.";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"Genre with ID {genreId} is not found";
            }

            return result;
        }

        public async Task<OperationResult> DeleteTrackAsync(string trackId)
        {
            var result = new OperationResult();

            var track = await _context.Tracks.FindAsync(trackId);
            if (track != null)
            {
                try
                {
                    _context.Tracks.Remove(track);
                    await _context.SaveChangesAsync();

                    await UploadFileHelper.DeleteFile(trackId!, "musicplay");
                    await UploadFileHelper.DeleteFile(trackId!, "musicphoto");

                    result.Success = true;
                    result.Message = $"Track '{track.Title}' and associated files deleted successfully";
                }
                catch (Exception ex)
                {
                    result.Success = false;
                    result.ErrorMessage = $"An error occurred: {ex.Message}";
                }
            }
            else
            {
                result.Success = false;
                result.ErrorMessage = $"Track with ID '{trackId}' is not found";
            }

            return result;
        }

        #endregion
    }
}
