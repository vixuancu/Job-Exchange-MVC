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

    /// <summary>
    /// ✅ Admin: Lấy tất cả Users với Pagination và Filters
    /// </summary>
    public async Task<PagedResultDto<ProfileDto>> GetAllUsersAsProfileDtoAsync(int page = 1, int pageSize = 10, string? role = null, string? status = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Users
                .Include(u => u.Company)
                .AsQueryable();

            // ✅ Apply filters BEFORE pagination
            if (!string.IsNullOrEmpty(role))
            {
                query = query.Where(u => u.Role == role);
            }

            if (!string.IsNullOrEmpty(status))
            {
                if (status == "active")
                {
                    query = query.Where(u => u.IsActive);
                }
                else if (status == "inactive")
                {
                    query = query.Where(u => !u.IsActive);
                }
            }

            // Order by CreatedAt
            query = query.OrderByDescending(u => u.CreatedAt);

            // Count total items AFTER filters
            var totalItems = await query.CountAsync();

            // Apply pagination
            var users = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var profileDtos = users.Select(u => new ProfileDto
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
            }).ToList();

            _logger.LogInformation("Retrieved {Count}/{Total} users (page {Page}) with filters: role={Role}, status={Status}",
                profileDtos.Count, totalItems, page, role ?? "all", status ?? "all");

            return new PagedResultDto<ProfileDto>
            {
                Items = profileDtos,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all users with pagination");
            return new PagedResultDto<ProfileDto>
            {
                Items = new List<ProfileDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
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

    /// <summary>
    /// ✅ FIX #3: Hard delete user (Admin only)
    /// Cascade: Delete Company, Jobs, Applications
    /// </summary>
    public async Task<bool> DeleteUserAsync(int userId)
    {
        try
        {
            var user = await _context.Users
                .Include(u => u.Company!)
                    .ThenInclude(c => c.Jobs)
                        .ThenInclude(j => j.Applications)
                .Include(u => u.Applications)
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                _logger.LogWarning("User {UserId} not found", userId);
                return false;
            }

            // Prevent deleting Admin users
            if (user.Role == "Admin")
            {
                _logger.LogWarning("Cannot delete Admin user {UserId}", userId);
                throw new InvalidOperationException("Không thể xóa tài khoản quản trị viên");
            }

            // Cascade delete for Employer
            if (user.Company != null)
            {
                // Delete all applications for employer's jobs
                var allApplications = user.Company.Jobs
                    .SelectMany(j => j.Applications)
                    .ToList();

                if (allApplications.Any())
                {
                    _context.Applications.RemoveRange(allApplications);
                    _logger.LogInformation("Deleting {Count} applications for employer {UserId}", allApplications.Count, userId);
                }

                // Delete all jobs
                if (user.Company.Jobs.Any())
                {
                    _context.Jobs.RemoveRange(user.Company.Jobs);
                    _logger.LogInformation("Deleting {Count} jobs for employer {UserId}", user.Company.Jobs.Count, userId);
                }

                // Delete company
                _context.Companies.Remove(user.Company);
                _logger.LogInformation("Deleting company for employer {UserId}", userId);
            }

            // Cascade delete for Applicant
            if (user.Applications.Any())
            {
                _context.Applications.RemoveRange(user.Applications);
                _logger.LogInformation("Deleting {Count} applications for applicant {UserId}", user.Applications.Count, userId);
            }

            // Delete user
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            _logger.LogInformation("User deleted successfully: {UserId} - {Email} - {Role}", userId, user.Email, user.Role);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user {UserId}", userId);
            throw;
        }
    }
}
