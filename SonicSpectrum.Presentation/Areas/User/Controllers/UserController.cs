using IdentityManagerServerApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Repository.Abstract;

namespace SonicSpectrum.Presentation.Areas.User.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController(IUnitOfWork _unitOfWork, JwtTokenService _jwtTokenService) : ControllerBase
    {

        [HttpGet("getMyId")]
        public async Task<IActionResult> GetUserInfo()
        {
            var authorizationHeader = HttpContext.Request.Headers["Authorization"].FirstOrDefault();
            if (authorizationHeader == null || !authorizationHeader.StartsWith("Bearer "))
                return Unauthorized("Token is missing or invalid.");

            var token = authorizationHeader.Substring("Bearer ".Length).Trim();

            try
            {
                var userId = _jwtTokenService.GetUserIdFromToken(token);

                return Ok(new { UserId = userId });
            }
            catch (Exception ex)
            {
                return Unauthorized($"Error extracting user ID: {ex.Message}");
            }
        }

        [HttpGet("getUserInfo/{userId}")]
        public async Task<IActionResult> GetUserInfo(string userId)
        {
            try
            {
                var userinfo = await _unitOfWork.AccountService.GetUserInfoAsync(userId);
                if (userinfo == null) return NotFound();
                return Ok(userinfo);
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

        [HttpGet("getUserFollowers/{userId}")]
        public async Task<IActionResult> GetUserFollowers(string userId)
        {
            try
            {
                var userfollowers = await _unitOfWork.AccountService.GetUserFollowers(userId);
                if (userfollowers == null) return NotFound();
                return Ok(userfollowers);
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

        [HttpGet("getUserFollowings/{userId}")]
        public async Task<IActionResult> GetUserFollings(string userId)
        {
            try
            {
                var userfollowings = await _unitOfWork.AccountService.GetUserFollowings(userId);
                if (userfollowings == null) return NotFound();
                return Ok(userfollowings);
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

