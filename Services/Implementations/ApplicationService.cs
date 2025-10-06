using Microsoft.EntityFrameworkCore;
using JobExchangeMvc.Data;
using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Services.Implementations;

public class ApplicationService : IApplicationService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<ApplicationService> _logger;

    public ApplicationService(ApplicationDbContext context, ILogger<ApplicationService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<ApplicationDto> CreateApplicationAsync(int applicantId, ApplicationDto applicationDto)
    {
        try
        {
            // Check if already applied
            var existingApplication = await _context.Applications
                .FirstOrDefaultAsync(a => a.JobId == applicationDto.JobId && a.ApplicantId == applicantId);

            if (existingApplication != null)
            {
                throw new InvalidOperationException("Bạn đã ứng tuyển vào công việc này rồi");
            }

            // Check if job exists and is active
            var job = await _context.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == applicationDto.JobId && j.IsActive && j.Status == "Approved");

            if (job == null)
            {
                throw new InvalidOperationException("Công việc không tồn tại hoặc không còn nhận ứng tuyển");
            }

            // Check application deadline
            if (job.ApplicationDeadline.HasValue && job.ApplicationDeadline.Value < DateTime.UtcNow)
            {
                throw new InvalidOperationException("Đã hết hạn nộp đơn ứng tuyển");
            }

            // Get applicant's CV if not provided
            var applicant = await _context.Users.FindAsync(applicantId);
            var cvUrl = !string.IsNullOrEmpty(applicationDto.CvUrl)
                ? applicationDto.CvUrl
                : applicant?.CvUrl;

            var application = new Application
            {
                JobId = applicationDto.JobId,
                ApplicantId = applicantId,
                CoverLetter = applicationDto.CoverLetter,
                CvUrl = cvUrl,
                Status = "Pending",
                AppliedAt = DateTime.UtcNow
            };

            await _context.Applications.AddAsync(application);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Application created: {ApplicationId} by applicant {ApplicantId} for job {JobId}",
                application.Id, applicantId, applicationDto.JobId);

            return new ApplicationDto
            {
                Id = application.Id,
                JobId = application.JobId,
                JobTitle = job.Title,
                CompanyName = job.Company.Name,
                CoverLetter = application.CoverLetter,
                CvUrl = application.CvUrl,
                Status = application.Status,
                AppliedAt = application.AppliedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating application for applicant {ApplicantId}", applicantId);
            throw;
        }
    }

    public async Task<ApplicationDto?> GetApplicationByIdAsync(int id)
    {
        try
        {
            var application = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Applicant)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (application == null)
            {
                return null;
            }

            return new ApplicationDto
            {
                Id = application.Id,
                JobId = application.JobId,
                JobTitle = application.Job.Title,
                CompanyName = application.Job.Company.Name,
                JobLocation = application.Job.Location,
                JobType = application.Job.JobType,
                SalaryRange = application.Job.SalaryRange,
                ApplicantName = application.Applicant.FullName,
                ApplicantEmail = application.Applicant.Email,
                CoverLetter = application.CoverLetter,
                CvUrl = application.CvUrl,
                Status = application.Status,
                Note = application.Note,
                AppliedAt = application.AppliedAt,
                ReviewedAt = application.ReviewedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting application {ApplicationId}", id);
            return null;
        }
    }

    public async Task<IEnumerable<ApplicationDto>> GetApplicationsByApplicantAsync(int applicantId)
    {
        try
        {
            var applications = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Where(a => a.ApplicantId == applicantId)
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CompanyName = a.Job.Company.Name,
                    JobLocation = a.Job.Location,
                    JobType = a.Job.JobType,
                    SalaryRange = a.Job.SalaryRange,
                    CoverLetter = a.CoverLetter,
                    CvUrl = a.CvUrl,
                    Status = a.Status,
                    Note = a.Note,
                    AppliedAt = a.AppliedAt,
                    ReviewedAt = a.ReviewedAt
                })
                .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for applicant {ApplicantId}", applicantId);
            return new List<ApplicationDto>();
        }
    }

    public async Task<IEnumerable<ApplicationDto>> GetApplicationsByJobAsync(int jobId)
    {
        try
        {
            var applications = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Applicant)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CompanyName = a.Job.Company.Name,
                    ApplicantName = a.Applicant.FullName,
                    ApplicantEmail = a.Applicant.Email,
                    CoverLetter = a.CoverLetter,
                    CvUrl = a.CvUrl,
                    Status = a.Status,
                    Note = a.Note,
                    AppliedAt = a.AppliedAt,
                    ReviewedAt = a.ReviewedAt
                })
                .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for job {JobId}", jobId);
            return new List<ApplicationDto>();
        }
    }

    public async Task<IEnumerable<ApplicationDto>> GetApplicationsByEmployerAsync(int employerId)
    {
        try
        {
            var applications = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Applicant)
                .Where(a => a.Job.Company.EmployerId == employerId)
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CompanyName = a.Job.Company.Name,
                    ApplicantName = a.Applicant.FullName,
                    ApplicantEmail = a.Applicant.Email,
                    CoverLetter = a.CoverLetter,
                    CvUrl = a.CvUrl,
                    Status = a.Status,
                    Note = a.Note,
                    AppliedAt = a.AppliedAt,
                    ReviewedAt = a.ReviewedAt
                })
                .ToListAsync();

            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for employer {EmployerId}", employerId);
            return new List<ApplicationDto>();
        }
    }

    public async Task<bool> UpdateApplicationStatusAsync(int applicationId, string status, string? note = null)
    {
        try
        {
            var application = await _context.Applications.FindAsync(applicationId);
            if (application == null)
            {
                return false;
            }

            // Validate status - All possible statuses in the workflow
            var validStatuses = new[] { "Pending", "Approved", "Interviewing", "Accepted", "Rejected", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                _logger.LogWarning("Invalid status: {Status} for application {ApplicationId}", status, applicationId);
                return false;
            }

            application.Status = status;
            application.Note = note;
            application.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Application status updated: {ApplicationId} - Status: {Status}", applicationId, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating application status for application {ApplicationId}", applicationId);
            return false;
        }
    }

    public async Task<bool> CancelApplicationAsync(int applicationId, int applicantId)
    {
        try
        {
            var application = await _context.Applications
                .FirstOrDefaultAsync(a => a.Id == applicationId && a.ApplicantId == applicantId);

            if (application == null)
            {
                return false;
            }

            // Chỉ cho phép hủy nếu đang pending
            if (application.Status != "Pending")
            {
                return false;
            }

            application.Status = "Cancelled";
            application.ReviewedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Application cancelled: {ApplicationId} by applicant {ApplicantId}", applicationId, applicantId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling application {ApplicationId}", applicationId);
            return false;
        }
    }

    public async Task<bool> HasAppliedAsync(int applicantId, int jobId)
    {
        return await _context.Applications
            .AnyAsync(a => a.ApplicantId == applicantId && a.JobId == jobId);
    }
}
