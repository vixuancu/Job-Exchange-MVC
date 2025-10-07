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
            // ✅ FIX #13: Use transaction to prevent race condition
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Check if already applied
                var existingApplication = await _context.Applications
                    .FirstOrDefaultAsync(a => a.JobId == applicationDto.JobId && a.ApplicantId == applicantId);

                if (existingApplication != null)
                {
                    throw new InvalidOperationException("Bạn đã ứng tuyển vào công việc này rồi");
                }

                // ✅ FIX #4: Enhanced job validation
                var job = await _context.Jobs
                    .Include(j => j.Company)
                    .FirstOrDefaultAsync(j => j.Id == applicationDto.JobId);

                if (job == null)
                {
                    throw new InvalidOperationException("Công việc không tồn tại");
                }

                // Validate job is active
                if (!job.IsActive)
                {
                    throw new InvalidOperationException("Tin tuyển dụng này đã bị đóng hoặc xóa");
                }

                // Validate job status
                if (job.Status == "Closed")
                {
                    throw new InvalidOperationException("Tin tuyển dụng này đã được nhà tuyển dụng đóng");
                }

                if (job.Status == "Expired")
                {
                    throw new InvalidOperationException("Tin tuyển dụng này đã hết hạn");
                }

                if (job.Status == "Rejected")
                {
                    throw new InvalidOperationException("Tin tuyển dụng này đã bị từ chối bởi quản trị viên");
                }

                if (job.Status != "Approved")
                {
                    throw new InvalidOperationException("Tin tuyển dụng này chưa được duyệt");
                }

                // Validate application deadline
                if (job.ApplicationDeadline.HasValue && job.ApplicationDeadline.Value < DateTime.UtcNow)
                {
                    throw new InvalidOperationException("Đã hết hạn nộp đơn ứng tuyển cho tin này");
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
                await transaction.CommitAsync();

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
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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

    /// <summary>
    /// ✅ Lấy Applications của Applicant với Pagination
    /// </summary>
    public async Task<PagedResultDto<ApplicationDto>> GetApplicationsByApplicantAsync(int applicantId, int page = 1, int pageSize = 10)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Where(a => a.ApplicantId == applicantId)
                .OrderByDescending(a => a.AppliedAt);

            // Count total items
            var totalItems = await query.CountAsync();

            // Apply pagination
            var applications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            _logger.LogInformation("Retrieved {Count}/{Total} applications for applicant {ApplicantId} (page {Page})",
                applications.Count, totalItems, applicantId, page);

            return new PagedResultDto<ApplicationDto>
            {
                Items = applications,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for applicant {ApplicantId}", applicantId);
            return new PagedResultDto<ApplicationDto>
            {
                Items = new List<ApplicationDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
    }

    /// <summary>
    /// ✅ Lấy Applications của Job với Pagination
    /// </summary>
    public async Task<PagedResultDto<ApplicationDto>> GetApplicationsByJobAsync(int jobId, int page = 1, int pageSize = 20)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Applicant)
                .Where(a => a.JobId == jobId)
                .OrderByDescending(a => a.AppliedAt);

            // Count total items
            var totalItems = await query.CountAsync();

            // Apply pagination
            var applications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            _logger.LogInformation("Retrieved {Count}/{Total} applications for job {JobId} (page {Page})",
                applications.Count, totalItems, jobId, page);

            return new PagedResultDto<ApplicationDto>
            {
                Items = applications,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for job {JobId}", jobId);
            return new PagedResultDto<ApplicationDto>
            {
                Items = new List<ApplicationDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
    }

    /// <summary>
    /// ✅ Lấy Applications của Employer với Pagination
    /// </summary>
    public async Task<PagedResultDto<ApplicationDto>> GetApplicationsByEmployerAsync(int employerId, int page = 1, int pageSize = 20)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Applicant)
                .Where(a => a.Job.Company.EmployerId == employerId)
                .OrderByDescending(a => a.AppliedAt);

            // Count total items
            var totalItems = await query.CountAsync();

            // Apply pagination
            var applications = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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

            _logger.LogInformation("Retrieved {Count}/{Total} applications for employer {EmployerId} (page {Page})",
                applications.Count, totalItems, employerId, page);

            return new PagedResultDto<ApplicationDto>
            {
                Items = applications,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting applications for employer {EmployerId}", employerId);
            return new PagedResultDto<ApplicationDto>
            {
                Items = new List<ApplicationDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
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

    /// <summary>
    /// ✅ NEW: Admin lấy TẤT CẢ Applications để tính statistics
    /// </summary>
    public async Task<IEnumerable<ApplicationDto>> GetAllApplicationsForAdminAsync()
    {
        try
        {
            var applications = await _context.Applications
                .Include(a => a.Job)
                    .ThenInclude(j => j.Company)
                .Include(a => a.Applicant)
                .OrderByDescending(a => a.AppliedAt)
                .Select(a => new ApplicationDto
                {
                    Id = a.Id,
                    JobId = a.JobId,
                    JobTitle = a.Job.Title,
                    CompanyName = a.Job.Company.Name,
                    ApplicantName = a.Applicant.FullName,
                    ApplicantEmail = a.Applicant.Email,
                    Status = a.Status,
                    AppliedAt = a.AppliedAt,
                    ReviewedAt = a.ReviewedAt
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count} applications for admin statistics", applications.Count);
            return applications;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all applications for admin");
            return new List<ApplicationDto>();
        }
    }
}
