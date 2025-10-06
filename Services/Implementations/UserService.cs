using Microsoft.EntityFrameworkCore;
using JobExchangeMvc.Data;
using JobExchangeMvc.DTOs;
using JobExchangeMvc.Helpers;
using JobExchangeMvc.Models;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Services.Implementations;

public class UserService : IUserService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<UserService> _logger;

    public UserService(ApplicationDbContext context, ILogger<UserService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<User?> GetUserByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.Company)
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<ProfileDto?> GetProfileAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return null;
            }

            var profileDto = new ProfileDto
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt,
                FullName = user.FullName,
                PhoneNumber = user.PhoneNumber,
                AvatarUrl = user.AvatarUrl,
                CvUrl = user.CvUrl,
                Skills = user.Skills,
                Bio = user.Bio
            };

            if (user.Role == "Employer" && user.Company != null)
            {
                profileDto.Company = new CompanyDto
                {
                    Id = user.Company.Id,
                    Name = user.Company.Name,
                    Description = user.Company.Description,
                    LogoUrl = user.Company.LogoUrl,
                    Website = user.Company.Website,
                    Address = user.Company.Address,
                    City = user.Company.City
                };
            }

            return profileDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting profile for user {UserId}", userId);
            return null;
        }
    }

    public async Task<bool> UpdateProfileAsync(int userId, ProfileDto profileDto)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Company)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return false;
            }

            // Update user info
            user.FullName = profileDto.FullName;
            user.PhoneNumber = profileDto.PhoneNumber;
            user.Skills = profileDto.Skills;
            user.Bio = profileDto.Bio;
            user.UpdatedAt = DateTime.UtcNow;

            // Update avatar URL if provided
            if (!string.IsNullOrEmpty(profileDto.AvatarUrl))
            {
                user.AvatarUrl = profileDto.AvatarUrl;
            }

            // Update CV URL if provided
            if (!string.IsNullOrEmpty(profileDto.CvUrl))
            {
                user.CvUrl = profileDto.CvUrl;
            }

            // Update company info for employers
            if (user.Role == "Employer" && profileDto.Company != null)
            {
                if (user.Company == null)
                {
                    // Create new company
                    var company = new Company
                    {
                        Name = profileDto.Company.Name,
                        Description = profileDto.Company.Description,
                        LogoUrl = profileDto.Company.LogoUrl,
                        Website = profileDto.Company.Website,
                        Address = profileDto.Company.Address,
                        City = profileDto.Company.City,
                        EmployerId = userId,
                        CreatedAt = DateTime.UtcNow
                    };
                    await _context.Companies.AddAsync(company);
                }
                else
                {
                    // Update existing company
                    user.Company.Name = profileDto.Company.Name;
                    user.Company.Description = profileDto.Company.Description;
                    user.Company.Website = profileDto.Company.Website;
                    user.Company.Address = profileDto.Company.Address;
                    user.Company.City = profileDto.Company.City;

                    if (!string.IsNullOrEmpty(profileDto.Company.LogoUrl))
                    {
                        user.Company.LogoUrl = profileDto.Company.LogoUrl;
                    }
                }
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Profile updated successfully for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating profile for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> ChangePasswordAsync(int userId, string currentPassword, string newPassword)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Verify current password
            if (!PasswordHasher.VerifyPassword(currentPassword, user.PasswordHash))
            {
                return false;
            }

            // Update password
            user.PasswordHash = PasswordHasher.HashPassword(newPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Password changed successfully for user {UserId}", userId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error changing password for user {UserId}", userId);
            return false;
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync()
    {
        return await _context.Users
            .Include(u => u.Company)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<ProfileDto>> GetAllUsersAsProfileDtoAsync()
    {
        var users = await _context.Users
            .Include(u => u.Company)
            .OrderByDescending(u => u.CreatedAt)
            .ToListAsync();

        return users.Select(u => new ProfileDto
        {
            Id = u.Id,
            Email = u.Email,
            FullName = u.FullName,
            PhoneNumber = u.PhoneNumber,
            Bio = u.Bio,
            Skills = u.Skills,
            AvatarUrl = u.AvatarUrl,
            CvUrl = u.CvUrl,
            Role = u.Role,
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt,
            CompanyName = u.Company?.Name,
            CompanyLogoUrl = u.Company?.LogoUrl
        });
    }

    public async Task<bool> UpdateUserStatusAsync(int userId, bool isActive)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.IsActive = isActive;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("User status updated: {UserId} - Active: {IsActive}", userId, isActive);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user status for user {UserId}", userId);
            return false;
        }
    }

    public async Task<bool> UpdateUserRoleAsync(int userId, string role)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return false;
            }

            // Validate role
            if (role != "Admin" && role != "Employer" && role != "Applicant")
            {
                return false;
            }

            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("User role updated: {UserId} - Role: {Role}", userId, role);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user role for user {UserId}", userId);
            return false;
        }
    }
}
