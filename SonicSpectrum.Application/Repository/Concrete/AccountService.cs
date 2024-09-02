using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SonicSpectrum.Application.DTOs;
using SonicSpectrum.Application.Models;
using SonicSpectrum.Application.Repository.Abstract;
using SonicSpectrum.Application.Services;
using SonicSpectrum.Domain.Entities;
using SonicSpectrum.Persistence.Data;

namespace SonicSpectrum.Application.Repository.Concrete
{
    public class AccountService(AppDbContext _context, UserManager<User> _userManager) : IAccountService
    {
        public async Task<OperationResult> ChangeNickname(string email, string newNickname)
        {
            var result = new OperationResult();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "User not found.";
                    return result;
                }

                user.UserName = newNickname;
                var identityResult = await _userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                {
                    result.Success = false;
                    result.ErrorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return result;
                }

                await transaction.CommitAsync();

                result.Success = true;
                result.Message = "Nickname updated successfully";
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

        public async Task<OperationResult> ChangeImage(ChangeImageDto imageDto)
        {
            var result = new OperationResult();


            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByEmailAsync(imageDto.Email);
                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "User not found.";
                    return result;
                }

                if (user.ImageUrl == $"https://seventysoundst.blob.core.windows.net/userphoto/{user.Id}")
                    await UploadFileHelper.DeleteFile(user.Id!, "userphoto");

                user.ImageUrl = await UploadFileHelper.UploadFile(imageDto.NewImage!, "userphoto", user.Id);
                var identityResult = await _userManager.UpdateAsync(user);

                if (!identityResult.Succeeded)
                {
                    result.Success = false;
                    result.ErrorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return result;
                }

                await transaction.CommitAsync();
                await transaction.DisposeAsync();

                result.Success = true;
                result.Message = "Image updated successfully.";
                return result;
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                await transaction.DisposeAsync();

                result.Success = false;
                result.ErrorMessage = $"An error occurred: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> OpenProfileAsync(string userId)
        {
            var result = new OperationResult();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "User don't find";
                    return result;
                }

                user.IsProfileOpen = true;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "Profile opened";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> CloseProfileAsync(string userId)
        {
            var result = new OperationResult();

            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "User don't find.";
                    return result;
                }

                user.IsProfileOpen = false;
                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                result.Success = true;
                result.Message = "profile succsesfuly privated.";
                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = $"Error: {ex.Message}";
                return result;
            }
        }

        public async Task<OperationResult> DeleteAccount(string userId)
        {
            var result = new OperationResult();

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "User not found.";
                    return result;
                }

                if (!string.IsNullOrEmpty(user.ImageUrl)) await UploadFileHelper.DeleteFile(userId, "userphoto");
                

                var identityResult = await _userManager.DeleteAsync(user);

                if (!identityResult.Succeeded)
                {
                    result.Success = false;
                    result.ErrorMessage = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                    return result;
                }

                await transaction.CommitAsync();

                result.Success = true;
                result.Message = "Account deleted successfully.";
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


        public async Task<object> GetUserInfoAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId), "user ID cannot be null or empty.");
            try
            {
                var user = await _context.Users.AsNoTracking()
                                                .Where(u => u.Id == userId)
                                                .Select(u => new
                                                {
                                                    u.Id,
                                                    u.UserName,
                                                    u.FullName,
                                                    u.Age,
                                                    u.ImageUrl,
                                                    u.Email,
                                                    u.IsProfileOpen,
                                                    u.EmailConfirmed,
                                                    FollowersCount = u.Followers.Count(f => f.RequestStatus == "Accepted"),
                                                    FollowingsCount = u.Followings.Count(f => f.RequestStatus == "Accepted"),
                                                    PlaylistsCount = u.Playlists!.Count(),
                                                    Playlists = u.Playlists!.Select(p => new { p.PlaylistId, p.Name, p.PlaylistImage })
                                                }).FirstOrDefaultAsync();

                if (user == null) throw new KeyNotFoundException($"User with ID '{userId}' not found.");

                return user;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
        }

        public async Task<IEnumerable<object>> GetUserFollowers(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId), "user ID cannot be null or empty.");
            try
            {
                var userFollowers = await _context.Users.AsNoTracking()
                                         .Where(u => u.Id == userId)
                                         .Select(u => new
                                         {
                                             Followers = u.Followers.Select(f => new { f.Id, f.FollowerId,f.Follower.UserName, f.Follower.FullName, f.Follower.ImageUrl, f.RequestStatus, f.RequestedDate }),
                                         }).ToListAsync();
                return userFollowers;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<object>> GetUserFollowings(string userId)
        {
            if (string.IsNullOrEmpty(userId)) throw new ArgumentNullException(nameof(userId), "user ID cannot be null or empty.");
            try
            {
                var userFollowings = await _context.Users.AsNoTracking()
                                         .Where(u => u.Id == userId)
                                         .Select(u => new
                                         {
                                             Followings = u.Followings.Select(f => new { f.Id, f.FolloweeId,f.Followee.UserName,f.Followee.FullName,f.Followee.ImageUrl,f.RequestStatus,f.RequestedDate }),
                                         }).ToListAsync();
                return userFollowings;
            }
            catch (Exception ex)
            {
                await Console.Out.WriteLineAsync(ex.Message);
                throw;
            }
            throw new NotImplementedException();
        }
    }
}
