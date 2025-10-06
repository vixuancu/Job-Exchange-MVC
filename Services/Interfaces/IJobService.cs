using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;

namespace JobExchangeMvc.Services.Interfaces;

public interface IJobService
{
    Task<IEnumerable<JobDto>> GetAllJobsAsync(string? searchTerm = null, int? categoryId = null, string? location = null);
    Task<JobDto?> GetJobByIdAsync(int id);
    Task<JobDto> CreateJobAsync(int employerId, JobDto jobDto);
    Task<bool> UpdateJobAsync(int jobId, int employerId, JobDto jobDto);
    Task<bool> DeleteJobAsync(int jobId, int employerId);
    Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(int employerId);
    Task<IEnumerable<JobDto>> GetJobsByCompanyAsync(int companyId);
    Task<bool> UpdateJobStatusAsync(int jobId, string status);
    Task<bool> IncrementViewCountAsync(int jobId);
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetAllCategoriesForAdminAsync();

    // Category management
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(string name, string? description);
    Task<bool> UpdateCategoryAsync(int id, string name, string? description, bool isActive);
    Task<bool> ToggleCategoryStatusAsync(int id, bool isActive);
}
