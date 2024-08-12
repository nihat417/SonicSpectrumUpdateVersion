using static SonicSpectrum.Application.Responses.ServiceResponses;
using SonicSpectrum.Application.DTOs;

namespace SonicSpectrum.Application.Repository.Abstract
{
    public interface IAuthService
    {
        Task<GeneralResponse> Register(RegisterDTO userDTO);
        Task<LoginResponse> Login(LoginDTO userDTO);
    }
}
