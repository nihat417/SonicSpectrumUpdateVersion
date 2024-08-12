using Microsoft.AspNetCore.Mvc;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Repository.Abstract;

namespace SonicSpectrum.Presentation.Areas.User.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUnitOfWork _unitOfWork) : ControllerBase
    {
        [HttpPost("follow")]
        public async Task<IActionResult> Follow([FromBody] FollowDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.FollowerId) || string.IsNullOrEmpty(dto.FolloweeId))
                return BadRequest("Invalid request data.");

            var result = await _unitOfWork.FollowService.FollowUserAsync(dto.FollowerId, dto.FolloweeId);
            if (result.Success) return Ok(result.Message);

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("accept")]
        public async Task<IActionResult> AcceptFollowRequest([FromBody] FollowDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.FollowerId) || string.IsNullOrEmpty(dto.FolloweeId))
                return BadRequest("Invalid request data.");

            var result = await _unitOfWork.FollowService.AcceptFollowRequestAsync(dto.FollowerId, dto.FolloweeId);
            if (result.Success) return Ok(result.Message);

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("unfollow")]
        public async Task<IActionResult> Unfollow([FromBody] FollowDto dto)
        {
            if (dto == null || string.IsNullOrEmpty(dto.FollowerId) || string.IsNullOrEmpty(dto.FolloweeId))
                return BadRequest("Invalid request data.");

            var result = await _unitOfWork.FollowService.UnfollowUserAsync(dto.FollowerId, dto.FolloweeId);
            if (result.Success) return Ok(result.Message);

            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("ChangeNickname")]
        public async Task<IActionResult> ChangeNickname(string email, string newNickname)
        {
            var result = await _unitOfWork.AccountService.ChangeNickname(email, newNickname);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("ChangeImage")]
        public async Task<IActionResult> ChangeImage([FromForm]ChangeImageDto imageDto)
        {
            var result = await _unitOfWork.AccountService.ChangeImage(imageDto);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpDelete("{userId}")]
        public async Task<IActionResult> DeleteAccount(string userId)
        {
            var result = await _unitOfWork.AccountService.DeleteAccount(userId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("OpenProfile")]
        public async Task<IActionResult> OpenProfile(string userId)
        {
            var result = await _unitOfWork.AccountService.OpenProfileAsync(userId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }

        [HttpPost("PrivateProfile")]
        public async Task<IActionResult> PrivateProfile(string userId)
        {
            var result = await _unitOfWork.AccountService.CloseProfileAsync(userId);
            if (result.Success) return Ok(result.Message);
            return BadRequest(result.ErrorMessage);
        }
    }
}

