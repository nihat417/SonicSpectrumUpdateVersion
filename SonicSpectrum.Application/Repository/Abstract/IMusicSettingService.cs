using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Models;
using SonicSpectrum.Domain.Entities;

namespace SonicSpectrum.Application.Repository.Abstract
{
    public  interface IMusicSettingService
    {

        #region Add

        Task<OperationResult> AddAlbumAsync(AlbumDto albumDto);
        Task<OperationResult> AddArtistAsync(ArtistDTO artistDto);
        Task<OperationResult> AddGenreAsync(GenreDTO genreDto);
        Task<OperationResult> AddTrackAsync(TrackDTO trackDto);
        Task<OperationResult> AddGenreToTrackAsync(string trackId, string genreName);
        Task<OperationResult> AddLyricsToTrackAsync(string trackId, string lyricsText);

        Task<OperationResult> CreatePlaylistAsync(PlaylistDTO requestDto);
        Task<OperationResult> AddTrackToPlaylistAsync(TrackPlaylistDTO requestDto);

        Task UpdateTrackListeningStatisticsAsync(string trackId, int minutesListened);
        Task UpdateUserListeningStatisticsAsync(string userId, string trackId, int minutesListened);

        #endregion

        #region edit

        Task<OperationResult> EditAlbumAsync(string albumId, AlbumDto albumDto);
        Task<OperationResult> EditArtistAsync(string artistId, ArtistDTO artistDto);
        Task<OperationResult> EditGenreAsync(string genreId, GenreDTO genreDto);
        Task<OperationResult> EditTrackAsync(string trackId, TrackDTO trackDto);

        #endregion

        #region Delete

        Task<OperationResult> DeleteArtistAsync(string artistId);
        Task<OperationResult> DeleteAlbumAsync(string albumId);
        Task<OperationResult> DeleteGenreAsync(string genreId);
        Task<OperationResult> DeleteTrackAsync(string trackId);

        #endregion

        #region get

        Task<object> GetTrackById(string id);

        Task<IEnumerable<object>> GetAllTracksAsync(int pageNumber, int pageSize);
        Task<IEnumerable<object>> GetAllArtistsAsync(int pageNumber, int pageSize);
        Task<IEnumerable<object>> GetAllAlbumsForArtistAsync(string artistId, int pageNumber, int pageSize);
        Task<IEnumerable<object>> GetMusicFromAlbum(string albumId, int pageNumber, int pageSize);
        Task<IEnumerable<object>> GetMusicFromPlaylist(string playlistId, int pageNumber, int pageSize);
        Task<IEnumerable<object>> GetPlaylistFromUser(string userId, int pageNumber, int pageSize);
        Task<object> SearchAsync(string query, int pageNumber, int pageSize);

        Task<IEnumerable<object>> GetRandomTracks();


        Task<IEnumerable<object>> GetRecommendedTracksAsync(string userId);
        Task<IEnumerable<object>> GetPopularArtistsAsync();
        Task<IEnumerable<object>> GetRecommendedAlbumsAsync();

        #endregion

    }
}
