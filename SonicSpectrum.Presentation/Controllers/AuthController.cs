using Microsoft.AspNetCore.Mvc;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Models;
using SonicSpectrum.Application.Repository.Abstract;

namespace SonicSpectrum.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IUnitOfWork _unitOfWork) : ControllerBase
    {

        #region AuthPost

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginDTO loginDTO)
        {
            var response = await _unitOfWork.AuthService.Login(loginDTO);
            return Ok(response);
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterDTO registerDTO)
        {
            var response = await _unitOfWork.AuthService.Register(registerDTO);
            if (response != null && response.Flag == true)
            {
                var user = await _unitOfWork.UserManager.FindByEmailAsync(registerDTO.Email);
                if (user != null)
                {
                    var token = await _unitOfWork.UserManager.GenerateEmailConfirmationTokenAsync(user!);
                    var confirmLink = Url.Action("ConfirmEmail", "Auth", new { token, email = registerDTO.Email }, Request.Scheme);
                    var message = new Message(new string[] { registerDTO.Email }, "Confirmation Email Link", confirmLink!);
                    _unitOfWork.EmailService.SendEmail(message);
                    return Ok(response);
                }
                return BadRequest();
            }
            return BadRequest(response);
        }

        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null || !(await _unitOfWork.UserManager.IsEmailConfirmedAsync(user)))
                return BadRequest("User not found or email is not confirmed.");

            var token = await _unitOfWork.UserManager.GeneratePasswordResetTokenAsync(user);
            var resetLink = Url.Action("ResetPassword", "Auth", new { token, email }, Request.Scheme);
            var message = new Message(new string[] { email }, "Reset Password Link", resetLink!);
            _unitOfWork.EmailService.SendEmail(message);

            return Ok("Password reset link has been sent to your email.");
        }

        [HttpPost("ResetPassword")]
        public async Task<IActionResult> ResetPassword(string token, string email, string newPassword)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("User not found.");

            var result = await _unitOfWork.UserManager.ResetPasswordAsync(user, token, newPassword);
            if (result.Succeeded)
                return Ok("Password has been reset successfully.");
            else
                return BadRequest("Failed to reset password.");
        }

        [HttpPost("ChangePassword")]
        public async Task<IActionResult> ChangePassword(string email, string currentPassword, string newPassword)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("User not found.");

            var passwordCheckResult = await _unitOfWork.UserManager.CheckPasswordAsync(user, currentPassword);
            if (!passwordCheckResult)
                return BadRequest("Current password is incorrect.");

            var changePasswordResult = await _unitOfWork.UserManager.ChangePasswordAsync(user, currentPassword, newPassword);
            if (changePasswordResult.Succeeded)
                return Ok("Password has been changed successfully.");
            else
                return BadRequest("Failed to change password.");
        }

        #endregion

        #region UpdateUser


        

        /*[HttpPost("ChangeProfilePhoto")]
        public async Task<IActionResult> ChangeProfilePhoto([FromForm] IFormFile photo, string email)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest("Invalid photo.");

            var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
            if (user == null)
                return NotFound("User not found.");

            var filePath = Path.Combine("wwwroot", "images", user.Id + Path.GetExtension(photo.FileName));
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            user.ImageUrl = filePath;
            var result = await _unitOfWork.UserManager.UpdateAsync(user);
            if (result.Succeeded)
                return Ok("Profile photo updated successfully.");
            else
                return BadRequest("Failed to update profile photo.");
        }

        [HttpPost("ChangeEmail")]
        public async Task<IActionResult> ChangeEmail(string currentEmail, string newEmail)
        {
            var user = await _unitOfWork.UserManager.FindByEmailAsync(currentEmail);
            if (user == null)
                return NotFound("User not found.");

            var token = await _unitOfWork.UserManager.GenerateChangeEmailTokenAsync(user, newEmail);
            var result = await _unitOfWork.UserManager.ChangeEmailAsync(user, newEmail, token);
            if (result.Succeeded)
                return Ok("Email updated successfully.");
            else
                return BadRequest("Failed to update email.");
        }*/


        #endregion

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string token, string email)
        {
            try
            {
                var user = await _unitOfWork.UserManager.FindByEmailAsync(email);
                if (user == null)
                    return NotFound();

                var result = await _unitOfWork.UserManager.ConfirmEmailAsync(user, token);
                if (result.Succeeded)
                    return Ok();
                else
                    return BadRequest("Failed to confirm email");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}
