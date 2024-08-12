using IdentityManagerServerApi.Services;
using Microsoft.AspNetCore.Identity;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Application.Services;
using SonicSpectrum.Application.UserSessions;
using SonicSpectrum.Domain.Entities;
using static SonicSpectrum.Application.Responses.ServiceResponses;

namespace SonicSpectrum.Application.Repository.Concrete
{
    public class AuthService (UserManager<User> userManager,RoleManager<IdentityRole> roleManager, JwtTokenService _jwtTokenService) : IAuthService
    {
        public async Task<LoginResponse> Login(LoginDTO userDTO)
        {
            if (userDTO == null)  return new LoginResponse(false, null!,null!,"Login model is empty");

            var getUser = await userManager.FindByEmailAsync(userDTO.Email);

            if (getUser == null)
                return new LoginResponse(false, null!,null!,"User not found");

            bool checkUserPasswords = await userManager.CheckPasswordAsync(getUser, userDTO.Password);
            if (!checkUserPasswords)
                return new LoginResponse(false, null!,null!,"Invalid email/password");

            var getUserRole = await userManager.GetRolesAsync(getUser);
            var userSession = new UserSession(getUser.Id, getUser.UserName,getUser.FullName,getUser.Age,getUser.Email, getUserRole.First());
            (string accsesToken,string refreshToken )= _jwtTokenService.CreateToken(userSession);
            return new LoginResponse(true, accsesToken!,refreshToken, "Login completed");
        }

        public async Task<GeneralResponse> Register(RegisterDTO userDTO)
        {
            if (userDTO == null) return new GeneralResponse(false, "Register DTO is Empty");
            var newUser = new User()
            {
                UserName = userDTO.UserName,
                FullName = userDTO.FullName,
                Email = userDTO.Email,
                IsProfileOpen = true,
                Age = userDTO.Age,
                PasswordHash = userDTO.Password,
                CreatedTime = DateTime.Now,
            };

            newUser.ImageUrl = (userDTO.ImageUrl != null) ? await UploadFileHelper.UploadFile(userDTO.ImageUrl!, "userphoto", newUser.Id) :
                   "https://seventysoundst.blob.core.windows.net/userphoto/userdef.png";

            var user = await userManager.FindByEmailAsync(userDTO.Email);
            if (user != null) return new GeneralResponse(false, "This email already registered");

            var userUsername = await userManager.FindByNameAsync(userDTO.UserName);
            if (userUsername != null) return new GeneralResponse(false, "This Username already registered");

            var createUser = await userManager.CreateAsync(newUser!, userDTO.Password);
            if (!createUser.Succeeded) return new GeneralResponse(false, "Error occured.. please try again");

            var checkUser = await roleManager.FindByNameAsync("User");
            if (checkUser is null)
                await roleManager.CreateAsync(new IdentityRole() { Name = "User" });

            await userManager.AddToRoleAsync(newUser, "User");
            return new GeneralResponse(true, "Account Created");
        }
    }
}
