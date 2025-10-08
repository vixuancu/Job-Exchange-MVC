using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;

namespace JobExchangeMvc.Services.Interfaces;

public interface IUserService
{
    Task<User?> GetUserByIdAsync(int id);
    Task<User?> GetUserByEmailAsync(string email);
    Task<ProfileDto?> GetProfileAsync(int userId);
    Task<bool> UpdateProfileAsync(int userId, ProfileDto profileDto);
    Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword);

    // Admin Methods with Pagination
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<PagedResultDto<ProfileDto>> GetAllUsersAsProfileDtoAsync(int page = 1, int pageSize = 10, string? role = null, string? status = null);
    Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
    Task<bool> UpdateUserRoleAsync(int userId, string role);
    Task<bool> DeleteUserAsync(int userId); // ✅ NEW: Hard delete user (admin only)
}
