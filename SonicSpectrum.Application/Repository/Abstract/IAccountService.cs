using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Models;

namespace SonicSpectrum.Application.Repository.Abstract
{
    public interface IAccountService
    {
        Task<OperationResult> ChangeNickname(string email, string newNickname);
        Task<OperationResult> ChangeImage(ChangeImageDto imageDto);
        Task<OperationResult> DeleteAccount(string userId);
        Task<OperationResult> OpenProfileAsync(string userId);
        Task<OperationResult> CloseProfileAsync(string userId);
    }
}
