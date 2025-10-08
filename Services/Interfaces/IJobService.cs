using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;

namespace JobExchangeMvc.Services.Interfaces;

public interface IJobService
{
    // Public Methods with Pagination
    Task<PagedResultDto<JobDto>> GetAllJobsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? categoryId = null, string? location = null);

    // Admin Methods with Pagination
    Task<PagedResultDto<JobDto>> GetAllJobsForAdminAsync(int page = 1, int pageSize = 20, string? status = null); // ✅ Admin: Lấy Jobs ACTIVE (IsActive = true) với filter
    Task<PagedResultDto<JobDto>> GetAllJobsIncludingDeletedAsync(int page = 1, int pageSize = 20); // ✅ NEW: Admin xem cả jobs đã xóa
    Task<JobDto?> GetJobByIdAsync(int id);
    Task<JobDto> CreateJobAsync(int employerId, JobDto jobDto);
    Task<bool> UpdateJobAsync(int jobId, int employerId, JobDto jobDto);
    Task<bool> DeleteJobAsync(int jobId, int employerId); // Soft delete (IsActive=false)
    Task<bool> HardDeleteJobAsync(int jobId); // ✅ NEW: Admin hard delete (remove from DB)
    Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(int employerId);
    Task<PagedResultDto<JobDto>> GetJobsByEmployerWithPaginationAsync(int employerId, int page = 1, int pageSize = 20); // ✅ NEW: Pagination for employer's jobs
    Task<IEnumerable<JobDto>> GetJobsByCompanyAsync(int companyId);
    Task<bool> UpdateJobStatusAsync(int jobId, string status);
    Task<bool> IncrementViewCountAsync(int jobId, int? userId = null, string? ipAddress = null, string? userAgent = null); // ✅ FIX #14: Updated signature
    Task<int> ExpireJobsPastDeadlineAsync(); // ✅ NEW: Batch expire jobs
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetAllCategoriesForAdminAsync();

    // Category management
    Task<Category?> GetCategoryByIdAsync(int id);
    Task<Category> CreateCategoryAsync(string name, string? description);
    Task<bool> UpdateCategoryAsync(int id, string name, string? description, bool isActive);
    Task<bool> ToggleCategoryStatusAsync(int id, bool isActive);
    Task<bool> DeleteCategoryAsync(int id); // ✅ NEW: Delete category with validation
}
