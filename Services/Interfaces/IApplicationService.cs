using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;

namespace JobExchangeMvc.Services.Interfaces;

public interface IApplicationService
{
    Task<ApplicationDto> CreateApplicationAsync(int applicantId, ApplicationDto applicationDto);
    Task<ApplicationDto?> GetApplicationByIdAsync(int id);
    Task<IEnumerable<ApplicationDto>> GetApplicationsByApplicantAsync(int applicantId);
    Task<IEnumerable<ApplicationDto>> GetApplicationsByJobAsync(int jobId);
    Task<IEnumerable<ApplicationDto>> GetApplicationsByEmployerAsync(int employerId);
    Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string? note = null);
    Task<bool> CancelApplicationAsync(int applicationId, int applicantId);
    Task<bool> HasAppliedAsync(int applicantId, int jobId);
}
