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
    Task<IEnumerable<User>> GetAllUsersAsync();
    Task<IEnumerable<ProfileDto>> GetAllUsersAsProfileDtoAsync();
    Task<bool> UpdateUserStatusAsync(int userId, bool isActive);
    Task<bool> UpdateUserRoleAsync(int userId, string role);
}
