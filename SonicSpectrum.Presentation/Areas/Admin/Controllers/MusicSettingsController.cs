using Microsoft.AspNetCore.Mvc;
using SonicSpectrum.Application;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Application.Repository.Concrete;

namespace SonicSpectrum.Presentation.Areas.Admin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MusicSettingsController(IUnitOfWork _unitOfWork) : ControllerBase
    {

        #region Post

        [HttpPost("addArtist")]
        public async Task<IActionResult> AddArtist([FromForm] ArtistDTO artistDto)
        {
            var result = await _unitOfWork.MusicSettingService.AddArtistAsync(artistDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("addAlbum")]
        public async Task<IActionResult> AddAlbum([FromForm] AlbumDto albumDto)
        {
            var result = await _unitOfWork.MusicSettingService.AddAlbumAsync(albumDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("addGenre")]
        public async Task<IActionResult> AddGenre([FromForm] GenreDTO genreDto)
        {
            var result = await _unitOfWork.MusicSettingService.AddGenreAsync(genreDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("addTrack")]
        public async Task<IActionResult> AddTrack([FromForm] TrackDTO trackDto)
        {
            var result = await _unitOfWork.MusicSettingService.AddTrackAsync(trackDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("addGenreToTrack/{trackId}/{genreName}")]
        public async Task<IActionResult> AddGenreToTrack(string trackId, string genreName)
        {
            var operationResult = await _unitOfWork.MusicSettingService.AddGenreToTrackAsync(trackId, genreName);

            if (operationResult.Success) return Ok(operationResult.Message);
            else return BadRequest(operationResult.ErrorMessage);
        }


        [HttpPost("addLyricsToTrack/{trackId}")]
        public async Task<IActionResult> AddLyricsToTrack(string trackId, [FromBody] string lyricsText)
        {
            if (string.IsNullOrEmpty(lyricsText))return BadRequest("Lyrics text is null or empty.");
            var operationResult = await _unitOfWork.MusicSettingService.AddLyricsToTrackAsync(trackId, lyricsText);
            if (operationResult.Success) return Ok(operationResult.Message);
            else return BadRequest(operationResult.ErrorMessage);
        }

       


        #endregion

        #region Put

        [HttpPut("editAlbum/{albumId}")]
        public async Task<IActionResult> EditAlbum(string albumId, [FromBody] AlbumDto albumDto)
        {
            var result = await _unitOfWork.MusicSettingService.EditAlbumAsync(albumId, albumDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("editArtist/{artistId}")]
        public async Task<IActionResult> EditArtist(string artistId, [FromBody] ArtistDTO artistDto)
        {
            var result = await _unitOfWork.MusicSettingService.EditArtistAsync(artistId, artistDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("editGenre/{genreId}")]
        public async Task<IActionResult> EditGenre(string genreId, [FromBody] GenreDTO genreDto)
        {
            var result = await _unitOfWork.MusicSettingService.EditGenreAsync(genreId, genreDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPut("editTrack/{trackId}")]
        public async Task<IActionResult> EditTrack(string trackId, [FromForm] TrackDTO trackDto)
        {
            var result = await _unitOfWork.MusicSettingService.EditTrackAsync(trackId, trackDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }


        #endregion

        #region delete

        [HttpDelete("deleteAlbum/{albumId}")]
        public async Task<IActionResult> DeleteAlbum(string albumId)
        {
            var result = await _unitOfWork.MusicSettingService.DeleteAlbumAsync(albumId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("deleteTrack/{trackId}")]
        public async Task<IActionResult> DeleteTrack(string trackId)
        {
            var result = await _unitOfWork.MusicSettingService.DeleteTrackAsync(trackId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("deleteArtist/{artistId}")]
        public async Task<IActionResult> DeleteArtist(string artistId)
        {
            var result = await _unitOfWork.MusicSettingService.DeleteArtistAsync(artistId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("deleteGenre/{genreId}")]
        public async Task<IActionResult> DeleteGenre(string genreId)
        {
            var result = await _unitOfWork.MusicSettingService.DeleteGenreAsync(genreId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        #endregion

    }
}
