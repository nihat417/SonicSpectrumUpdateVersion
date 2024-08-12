using Microsoft.EntityFrameworkCore;
using SonicSpectrum.Application.Models;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Domain.Entities;
using SonicSpectrum.Persistence.Data;

namespace SonicSpectrum.Application.Repository.Concrete
{
    public class FollowService(AppDbContext _context):IFollowService
    {
        public async Task<OperationResult> FollowUserAsync(string followerId, string followeeId)
        {
            var result = new OperationResult();

            if (followerId == followeeId)
            {
                result.Success = false;
                result.ErrorMessage = "You can't follow yourself.";
                return result;
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var followee = await _context.Users.FirstOrDefaultAsync(u => u.Id == followeeId);
                if (followee == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "The user was not found.";
                    return result;
                }

                var existingFollow = await _context.Follows
                                                    .AsNoTracking()
                                                    .FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

                if (existingFollow != null)
                {
                    if (existingFollow.RequestStatus == "Pending")
                    {
                        result.Success = false;
                        result.ErrorMessage = "The subscription request is already waiting.";
                        return result;
                    }
                    else if (existingFollow.RequestStatus == "Accepted")
                    {
                        result.Success = false;
                        result.ErrorMessage = "You have already subscribed to this user.";
                        return result;
                    }
                }

                var follow = new Follow
                {
                    FollowerId = followerId,
                    FolloweeId = followeeId,
                    RequestedDate = DateTime.UtcNow,
                    RequestStatus = followee.IsProfileOpen ? "Accepted" : "Pending",
                    AcceptedDate = followee.IsProfileOpen ? DateTime.UtcNow : (DateTime?)null
                };

                await _context.Follows.AddAsync(follow);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                result.Success = true;
                result.Message = followee.IsProfileOpen
                    ? "You are now subscribed to this user."
                    : "The subscription request has been sent successfully.";

                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"Error: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> AcceptFollowRequestAsync(string followerId, string followeeId)
        {
            var result = new OperationResult();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId && f.RequestStatus == "Pending");

                if (follow == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Follow request not found.";
                    return result;
                }

                follow.RequestStatus = "Accepted";
                follow.AcceptedDate = DateTime.UtcNow;

                _context.Follows.Update(follow);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                result.Success = true;
                result.Message = "Follow request accepted successfully.";
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> UnfollowUserAsync(string followerId, string followeeId)
        {
            var result = new OperationResult();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var follow = await _context.Follows.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId && f.RequestStatus == "Accepted" || f.RequestStatus == "Pending");

                if (follow == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "You are not following this user.";
                    return result;
                }

                _context.Follows.Remove(follow);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                result.Success = true;
                result.Message = "Unfollowed user successfully.";
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }
    }
}
