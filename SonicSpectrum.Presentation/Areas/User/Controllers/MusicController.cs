using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Application.Repository.Concrete;
using SonicSpectrum.Domain.Entities;

namespace SonicSpectrum.Presentation.Areas.User.Controllers
{

    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MusicController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        #region getmethods

        [HttpGet("getTrackById/{trackId}")]
        public async Task<IActionResult> GetTrackById(string trackId)
        {
            try
            {
                var music = await _unitOfWork.MusicSettingService.GetTrackById(trackId);
                if (music == null) return NotFound();
                return Ok(music);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getArtistById/{artistId}")]
        public async Task<IActionResult> GetArtistById(string artistId)
        {
            try
            {
                var artist = await _unitOfWork.MusicSettingService.GetArtistById(artistId);
                if (artist == null) return NotFound();
                return Ok(artist);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getalbumInfo/{albumId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAlbumInfo(string albumId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var albums = await _unitOfWork.MusicSettingService.GetAlbumInfo(albumId, pageNumber, pageSize);
                if (albums == null || !albums.Any()) return NotFound();
                return Ok(albums);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("GetMusicForAlbum/{albumId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetMusicForAlbum(string albumId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var tracks = await _unitOfWork.MusicSettingService.GetMusicFromAlbum(albumId, pageNumber, pageSize);
                if (tracks == null) return NotFound();
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("getallalbumsforartist/{artistId}")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllAlbumsForArtist(string artistId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var albums = await _unitOfWork.MusicSettingService.GetAllAlbumsForArtistAsync(artistId, pageNumber, pageSize);
                if (albums == null || !albums.Any()) return NotFound();
                return Ok(albums);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("user/{userId}/playlists")]
        public async Task<IActionResult> GetPlaylistsFromUser(string userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var playlists = await _unitOfWork.MusicSettingService.GetPlaylistFromUser(userId, pageNumber, pageSize);
                if (playlists == null) return NotFound();
                else return Ok(playlists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");

            }
        }

        [HttpGet("playlist/{playlistId}/tracks")]
        public async Task<IActionResult> GetMusicFromPlaylist(string playlistId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var tracks = await _unitOfWork.MusicSettingService.GetMusicFromPlaylist(playlistId, pageNumber, pageSize);
                if (tracks == null) return NotFound();
                else return Ok(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpGet("geAllInfoPlaylistById/{playlistId}")]
        public async Task<IActionResult> GetAllInfoPlaylistById(string playlistId,int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var tracks = await _unitOfWork.MusicSettingService.GetAllInfoPlaylistById(playlistId, pageNumber, pageSize);
                if (tracks == null) return NotFound();
                else return Ok(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

    [HttpGet("allmusics")]
        public async Task<IActionResult> GetAllMusics(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var tracks = await _unitOfWork.MusicSettingService.GetAllTracksAsync(pageNumber, pageSize);
                return Ok(tracks);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("search")]
        public async Task<IActionResult> SearchAsync([FromQuery] string query, [FromQuery] string userId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(query))
                return BadRequest("Search query cannot be null or empty.");

            try
            {
                var results = await _unitOfWork.MusicSettingService.SearchAsync(query, pageNumber, pageSize, userId);
                return Ok(results);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error occurred while searching: {ex.Message}");
                return StatusCode(500, "An error occurred while processing your request.");
            }
        }

        [HttpGet("randomtracks")]
        public async Task<IActionResult> GetRandomMusic()
        {
            try
            {
                var randomTracks = await _unitOfWork.MusicSettingService.GetRandomTracks();
                return Ok(randomTracks);
            }
            catch(Exception ex)
            {
                return StatusCode(500, "Internal server error: " + ex.Message);
            }
        }

        [HttpGet("allartists")]
        public async Task<ActionResult<IEnumerable<object>>> GetAllArtists(int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var artists = await _unitOfWork.MusicSettingService.GetAllArtistsAsync(pageNumber, pageSize);
                if (artists == null) return NotFound();
                return Ok(artists);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
        
        [HttpGet("recomendedTracs")]
        public async Task<IActionResult> GetRecommendedTracks(string userId)
        {
            try
            {
                var recommendedTracks = await _unitOfWork.MusicSettingService.GetRecommendedTracksAsync(userId);
                return Ok(recommendedTracks);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("popularArtist")]
        public async Task<IActionResult> GetPopularArtists()
        {
            try
            {
                var popularArtists = await _unitOfWork.MusicSettingService.GetPopularArtistsAsync();
                return Ok(popularArtists);
            }
            catch(Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("recommendedAlbums")]
        public async Task<IActionResult> GetRecommendedAlbums()
        {
            try
            {
                var recommendedAlbums = await _unitOfWork.MusicSettingService.GetRecommendedAlbumsAsync();
                return Ok(recommendedAlbums);
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                return BadRequest(ex.Message);
            }
        }

        #endregion

        #region postmethods

        [HttpPost("createPlaylist")]
        public async Task<IActionResult> CreatePlaylistAsync([FromForm] PlaylistDTO requestDto)
        {
            try
            {
                var result = await _unitOfWork.MusicSettingService.CreatePlaylistAsync(requestDto);
                if (result.Success) return Ok(result.Message);
                else return BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("addTrackPlaylist")]
        public async Task<IActionResult> AddTrackToPlaylistAsync([FromBody] TrackPlaylistDTO requestDto)
        {
            try
            {
                var result = await _unitOfWork.MusicSettingService.AddTrackToPlaylistAsync(requestDto);
                if (result.Success) return Ok(result.Message);
                else return BadRequest(result.ErrorMessage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        [HttpPost("updateTrackStatistics")]
        public async Task<IActionResult> UpdateTrackStatistics([FromBody] UpdateTrackStatisticsDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.TrackId) || dto.MinutesListened <= 0)
                return BadRequest("Invalid request data");

            await _unitOfWork.MusicSettingService.UpdateTrackListeningStatisticsAsync(dto.TrackId, dto.MinutesListened);
            return Ok("Track statistics updated successfully");
        }

        [HttpPost("updateUserStatistics")]
        public async Task<IActionResult> UpdateUserStatistics([FromBody] UpdateUserStatisticsDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.UserId) || string.IsNullOrEmpty(dto.TrackId) || dto.MinutesListened <= 0)
                return BadRequest("Invalid request data");

            await _unitOfWork.MusicSettingService.UpdateUserListeningStatisticsAsync(dto.UserId, dto.TrackId, dto.MinutesListened);
            return Ok("User statistics updated successfully");
        }

        #endregion
    }
}
