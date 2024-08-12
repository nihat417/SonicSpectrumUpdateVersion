using SonicSpectrum.Application.Models;

namespace SonicSpectrum.Application.Repository.Abstract
{
    public interface IFollowService
    {
        Task<OperationResult> FollowUserAsync(string followerId, string followeeId);
        Task<OperationResult> AcceptFollowRequestAsync(string followerId, string followeeId); 
        Task<OperationResult> UnfollowUserAsync(string followerId, string followeeId);
    }
}
