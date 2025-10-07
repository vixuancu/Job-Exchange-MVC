using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;

namespace JobExchangeMvc.Services.Interfaces;

public interface IApplicationService
{
    Task<ApplicationDto> CreateApplicationAsync(int applicantId, ApplicationDto applicationDto);
    Task<ApplicationDto?> GetApplicationByIdAsync(int id);

    // Methods with Pagination
    Task<PagedResultDto<ApplicationDto>> GetApplicationsByApplicantAsync(int applicantId, int page = 1, int pageSize = 10);
    Task<PagedResultDto<ApplicationDto>> GetApplicationsByJobAsync(int jobId, int page = 1, int pageSize = 20);
    Task<PagedResultDto<ApplicationDto>> GetApplicationsByEmployerAsync(int employerId, int page = 1, int pageSize = 20);

    // Admin Methods
    Task<IEnumerable<ApplicationDto>> GetAllApplicationsForAdminAsync(); // ✅ NEW: For admin statistics

    Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string? note = null);
    Task<bool> CancelApplicationAsync(int applicationId, int applicantId);
    Task<bool> HasAppliedAsync(int applicantId, int jobId);
}
