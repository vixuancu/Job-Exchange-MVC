using Microsoft.EntityFrameworkCore;
using JobExchangeMvc.Data;
using JobExchangeMvc.DTOs;
using JobExchangeMvc.Models;
using JobExchangeMvc.Services.Interfaces;

namespace JobExchangeMvc.Services.Implementations;

public class JobService : IJobService
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<JobService> _logger;

    public JobService(ApplicationDbContext context, ILogger<JobService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// ✅ Public: Lấy Jobs đã duyệt với Pagination
    /// </summary>
    public async Task<PagedResultDto<JobDto>> GetAllJobsAsync(int page = 1, int pageSize = 10, string? searchTerm = null, int? categoryId = null, string? location = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Max 100 items per page

            var query = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Where(j => j.IsActive && j.Status == "Approved")
                .AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(j =>
                    j.Title.Contains(searchTerm) ||
                    j.Description.Contains(searchTerm) ||
                    j.Company.Name.Contains(searchTerm));
            }

            if (categoryId.HasValue)
            {
                query = query.Where(j => j.CategoryId == categoryId.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(j => j.Location != null && j.Location.Contains(location));
            }

            // Count total items BEFORE pagination
            var totalItems = await query.CountAsync();

            // Apply pagination: Skip + Take
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Benefits = j.Benefits,
                    SalaryRange = j.SalaryRange,
                    Location = j.Location,
                    JobType = j.JobType,
                    NumberOfPositions = j.NumberOfPositions,
                    ApplicationDeadline = j.ApplicationDeadline,
                    Status = j.Status,
                    IsActive = j.IsActive,
                    ViewCount = j.ViewCount,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    CompanyLogoUrl = j.Company.LogoUrl,
                    CategoryId = j.CategoryId,
                    CategoryName = j.Category.Name,
                    CreatedAt = j.CreatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved page {Page}/{TotalPages} with {Count}/{Total} jobs",
                page, (int)Math.Ceiling((double)totalItems / pageSize), jobs.Count, totalItems);

            return new PagedResultDto<JobDto>
            {
                Items = jobs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting jobs with pagination");
            return new PagedResultDto<JobDto>
            {
                Items = new List<JobDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
    }

    /// <summary>
    /// ✅ Admin: Lấy Jobs ĐANG HOẠT ĐỘNG (IsActive = true) với Pagination và Filter
    /// Bao gồm tất cả Status: Pending, Approved, Rejected, Closed, Expired
    /// Loại trừ: Jobs đã bị soft delete (IsActive = false)
    /// </summary>
    public async Task<PagedResultDto<JobDto>> GetAllJobsForAdminAsync(int page = 1, int pageSize = 20, string? status = null)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Where(j => j.IsActive) // ✅ Chỉ lấy jobs đang active
                .AsQueryable();

            // ✅ Apply status filter BEFORE pagination
            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(j => j.Status == status);
            }

            // Count total items AFTER filters
            var totalItems = await query.CountAsync();

            // Apply pagination
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Benefits = j.Benefits,
                    SalaryRange = j.SalaryRange,
                    Location = j.Location,
                    JobType = j.JobType,
                    NumberOfPositions = j.NumberOfPositions,
                    ApplicationDeadline = j.ApplicationDeadline,
                    Status = j.Status,
                    IsActive = j.IsActive,
                    ViewCount = j.ViewCount,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    CompanyLogoUrl = j.Company.LogoUrl,
                    CategoryId = j.CategoryId,
                    CategoryName = j.Category.Name,
                    CreatedAt = j.CreatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count}/{Total} active jobs for admin (page {Page}) with status filter: {Status}",
                jobs.Count, totalItems, page, status ?? "all");

            return new PagedResultDto<JobDto>
            {
                Items = jobs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all jobs for admin");
            return new PagedResultDto<JobDto>
            {
                Items = new List<JobDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
    }

    /// <summary>
    /// ✅ NEW: Admin xem TẤT CẢ Jobs kể cả đã xóa (IsActive = false) với Pagination
    /// </summary>
    public async Task<PagedResultDto<JobDto>> GetAllJobsIncludingDeletedAsync(int page = 1, int pageSize = 20)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category);
            // KHÔNG filter IsActive → Lấy cả jobs đã xóa

            // Count total items
            var totalItems = await query.CountAsync();

            // Apply pagination
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Benefits = j.Benefits,
                    SalaryRange = j.SalaryRange,
                    Location = j.Location,
                    JobType = j.JobType,
                    NumberOfPositions = j.NumberOfPositions,
                    ApplicationDeadline = j.ApplicationDeadline,
                    Status = j.Status,
                    IsActive = j.IsActive,
                    ViewCount = j.ViewCount,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    CompanyLogoUrl = j.Company.LogoUrl,
                    CategoryId = j.CategoryId,
                    CategoryName = j.Category.Name,
                    CreatedAt = j.CreatedAt
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count}/{Total} jobs for admin including deleted (page {Page})",
                jobs.Count, totalItems, page);

            return new PagedResultDto<JobDto>
            {
                Items = jobs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting all jobs including deleted");
            return new PagedResultDto<JobDto>
            {
                Items = new List<JobDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
    }

    public async Task<JobDto?> GetJobByIdAsync(int id)
    {
        try
        {
            var job = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (job == null)
            {
                return null;
            }

            // ✅ FIX #2: Auto-expire job if deadline passed
            if (job.ApplicationDeadline.HasValue &&
                job.ApplicationDeadline.Value < DateTime.UtcNow &&
                job.Status == "Approved")
            {
                job.Status = "Expired";
                await _context.SaveChangesAsync();
                _logger.LogInformation("Auto-expired job {JobId} (Deadline: {Deadline})",
                    job.Id, job.ApplicationDeadline.Value);
            }

            return new JobDto
            {
                Id = job.Id,
                Title = job.Title,
                Description = job.Description,
                Requirements = job.Requirements,
                Benefits = job.Benefits,
                SalaryRange = job.SalaryRange,
                Location = job.Location,
                JobType = job.JobType,
                NumberOfPositions = job.NumberOfPositions,
                ApplicationDeadline = job.ApplicationDeadline,
                Status = job.Status,
                IsActive = job.IsActive,
                ViewCount = job.ViewCount,
                CompanyId = job.CompanyId,
                CompanyName = job.Company.Name,
                CompanyLogoUrl = job.Company.LogoUrl,
                CompanyDescription = job.Company.Description,
                CompanyWebsite = job.Company.Website,
                CompanyAddress = job.Company.Address,
                CompanyCity = job.Company.City,
                CompanyEmployerId = job.Company.EmployerId,
                CategoryId = job.CategoryId,
                CategoryName = job.Category.Name,
                CreatedAt = job.CreatedAt
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting job {JobId}", id);
            return null;
        }
    }

    public async Task<JobDto> CreateJobAsync(int employerId, JobDto jobDto)
    {
        try
        {
            // ✅ FIX #10: Enhanced Company validation
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.EmployerId == employerId);

            if (company == null)
            {
                throw new InvalidOperationException("Bạn cần tạo hồ sơ công ty trước khi đăng tin tuyển dụng. Vui lòng hoàn thiện thông tin công ty trong phần Hồ sơ.");
            }

            // Validate company has required information
            if (string.IsNullOrWhiteSpace(company.Name))
            {
                throw new InvalidOperationException("Tên công ty không được để trống. Vui lòng cập nhật thông tin công ty.");
            }

            if (string.IsNullOrWhiteSpace(company.Description))
            {
                throw new InvalidOperationException("Mô tả công ty không được để trống. Vui lòng bổ sung thông tin để tăng độ tin cậy với ứng viên.");
            }

            if (string.IsNullOrWhiteSpace(company.Address) || string.IsNullOrWhiteSpace(company.City))
            {
                throw new InvalidOperationException("Địa chỉ công ty không đầy đủ. Vui lòng cập nhật địa chỉ và thành phố.");
            }

            // Validate category exists and is active
            var category = await _context.Categories.FindAsync(jobDto.CategoryId);
            if (category == null || !category.IsActive)
            {
                throw new InvalidOperationException("Danh mục công việc không hợp lệ hoặc đã bị vô hiệu hóa. Vui lòng chọn danh mục khác.");
            }

            var job = new Job
            {
                Title = jobDto.Title,
                Description = jobDto.Description,
                Requirements = jobDto.Requirements,
                Benefits = jobDto.Benefits,
                SalaryRange = jobDto.SalaryRange,
                Location = jobDto.Location,
                JobType = jobDto.JobType,
                NumberOfPositions = jobDto.NumberOfPositions,
                ApplicationDeadline = jobDto.ApplicationDeadline,
                Status = "Pending", // Admin phải duyệt
                CompanyId = company.Id,
                CategoryId = jobDto.CategoryId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            await _context.Jobs.AddAsync(job);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Job created successfully: {JobId} by employer {EmployerId}", job.Id, employerId);

            jobDto.Id = job.Id;
            jobDto.CompanyId = job.CompanyId;
            jobDto.CompanyName = company.Name;
            jobDto.Status = job.Status;
            jobDto.CreatedAt = job.CreatedAt;

            return jobDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating job for employer {EmployerId}", employerId);
            throw;
        }
    }

    public async Task<bool> UpdateJobAsync(int jobId, int employerId, JobDto jobDto)
    {
        try
        {
            var job = await _context.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == jobId && j.Company.EmployerId == employerId);

            if (job == null)
            {
                return false;
            }

            // Update job details
            job.Title = jobDto.Title;
            job.Description = jobDto.Description;
            job.Requirements = jobDto.Requirements;
            job.Benefits = jobDto.Benefits;
            job.SalaryRange = jobDto.SalaryRange;
            job.Location = jobDto.Location;
            job.JobType = jobDto.JobType;
            job.NumberOfPositions = jobDto.NumberOfPositions;
            job.ApplicationDeadline = jobDto.ApplicationDeadline;
            job.CategoryId = jobDto.CategoryId;
            job.UpdatedAt = DateTime.UtcNow;

            // Reset to Pending if was Approved (changes need re-approval)
            if (job.Status == "Approved")
            {
                job.Status = "Pending";
            }

            await _context.SaveChangesAsync();
            _logger.LogInformation("Job updated successfully: {JobId}", jobId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job {JobId}", jobId);
            return false;
        }
    }

    public async Task<bool> DeleteJobAsync(int jobId, int employerId)
    {
        try
        {
            // ✅ FIX: Use simple Include for ownership check
            var job = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Applications) // ✅ Load Applications để cascade
                .FirstOrDefaultAsync(j => j.Id == jobId && j.Company.EmployerId == employerId && j.IsActive);

            if (job == null)
            {
                _logger.LogWarning("Job {JobId} not found, already deleted, or user {EmployerId} does not have permission", jobId, employerId);
                return false;
            }

            // ✅ FIX #1: Soft delete - Set IsActive = false + Status = "Closed"
            job.IsActive = false;
            job.Status = "Closed";
            job.UpdatedAt = DateTime.UtcNow;

            // ✅ FIX #7: Cascade to Applications - Reject tất cả applications đang Pending
            var pendingApplications = job.Applications.Where(a => a.Status == "Pending").ToList();
            foreach (var app in pendingApplications)
            {
                app.Status = "Rejected";
                app.ReviewedAt = DateTime.UtcNow;
                app.Note = "Tin tuyển dụng đã bị đóng bởi nhà tuyển dụng";
            }

            var changes = await _context.SaveChangesAsync();

            if (changes > 0)
            {
                _logger.LogInformation("Job {JobId} closed successfully by employer {EmployerId}. {Count} pending applications rejected.",
                    jobId, employerId, pendingApplications.Count);
                return true;
            }
            else
            {
                _logger.LogWarning("No changes were saved when deleting job {JobId}", jobId);
                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job {JobId} for employer {EmployerId}", jobId, employerId);
            return false;
        }
    }

    /// <summary>
    /// ✅ NEW: Admin hard delete job - XÓA HẲN khỏi database
    /// Chỉ dành cho Admin, xóa vĩnh viễn job và tất cả applications liên quan
    /// </summary>
    public async Task<bool> HardDeleteJobAsync(int jobId)
    {
        try
        {
            var job = await _context.Jobs
                .Include(j => j.Applications) // Load applications để cascade delete
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == jobId);

            if (job == null)
            {
                _logger.LogWarning("Job {JobId} not found for hard delete", jobId);
                return false;
            }

            // Log before delete
            _logger.LogWarning("HARD DELETE: Removing job {JobId} '{Title}' from database. Company: {CompanyName}, Applications: {AppCount}",
                job.Id, job.Title, job.Company?.Name, job.Applications.Count);

            // Remove from database (cascade delete applications)
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Job {JobId} permanently deleted from database", jobId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error hard deleting job {JobId}", jobId);
            return false;
        }
    }

    public async Task<IEnumerable<JobDto>> GetJobsByEmployerAsync(int employerId)
    {
        try
        {
            var jobs = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .Where(j => j.Company.EmployerId == employerId && j.IsActive) // ✅ FIX: Only show active jobs
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Benefits = j.Benefits,
                    SalaryRange = j.SalaryRange,
                    Location = j.Location,
                    JobType = j.JobType,
                    NumberOfPositions = j.NumberOfPositions,
                    ApplicationDeadline = j.ApplicationDeadline,
                    Status = j.Status,
                    ViewCount = j.ViewCount,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    CategoryId = j.CategoryId,
                    CategoryName = j.Category.Name,
                    CreatedAt = j.CreatedAt,
                    IsActive = j.IsActive,
                    ApplicationsCount = j.Applications.Count
                })
                .ToListAsync();

            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting jobs for employer {EmployerId}", employerId);
            return new List<JobDto>();
        }
    }

    /// <summary>
    /// ✅ NEW: Get jobs by employer with pagination
    /// </summary>
    public async Task<PagedResultDto<JobDto>> GetJobsByEmployerWithPaginationAsync(int employerId, int page = 1, int pageSize = 20)
    {
        try
        {
            // Validate pagination parameters
            if (page < 1) page = 1;
            if (pageSize < 1) pageSize = 20;
            if (pageSize > 100) pageSize = 100;

            var query = _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Include(j => j.Applications)
                .Where(j => j.Company.EmployerId == employerId && j.IsActive)
                .AsQueryable();

            // Count total items
            var totalItems = await query.CountAsync();

            // Apply pagination
            var jobs = await query
                .OrderByDescending(j => j.CreatedAt)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Benefits = j.Benefits,
                    SalaryRange = j.SalaryRange,
                    Location = j.Location,
                    JobType = j.JobType,
                    NumberOfPositions = j.NumberOfPositions,
                    ApplicationDeadline = j.ApplicationDeadline,
                    Status = j.Status,
                    ViewCount = j.ViewCount,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    CategoryId = j.CategoryId,
                    CategoryName = j.Category.Name,
                    CreatedAt = j.CreatedAt,
                    IsActive = j.IsActive,
                    ApplicationsCount = j.Applications.Count
                })
                .ToListAsync();

            _logger.LogInformation("Retrieved {Count}/{Total} jobs for employer {EmployerId} (page {Page})",
                jobs.Count, totalItems, employerId, page);

            return new PagedResultDto<JobDto>
            {
                Items = jobs,
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = totalItems
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting jobs with pagination for employer {EmployerId}", employerId);
            return new PagedResultDto<JobDto>
            {
                Items = new List<JobDto>(),
                CurrentPage = page,
                PageSize = pageSize,
                TotalItems = 0
            };
        }
    }

    public async Task<IEnumerable<JobDto>> GetJobsByCompanyAsync(int companyId)
    {
        try
        {
            var jobs = await _context.Jobs
                .Include(j => j.Company)
                .Include(j => j.Category)
                .Where(j => j.CompanyId == companyId && j.IsActive && j.Status == "Approved")
                .OrderByDescending(j => j.CreatedAt)
                .Select(j => new JobDto
                {
                    Id = j.Id,
                    Title = j.Title,
                    Description = j.Description,
                    Requirements = j.Requirements,
                    Benefits = j.Benefits,
                    SalaryRange = j.SalaryRange,
                    Location = j.Location,
                    JobType = j.JobType,
                    NumberOfPositions = j.NumberOfPositions,
                    ApplicationDeadline = j.ApplicationDeadline,
                    Status = j.Status,
                    ViewCount = j.ViewCount,
                    CompanyId = j.CompanyId,
                    CompanyName = j.Company.Name,
                    CategoryId = j.CategoryId,
                    CategoryName = j.Category.Name,
                    CreatedAt = j.CreatedAt
                })
                .ToListAsync();

            return jobs;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting jobs for company {CompanyId}", companyId);
            return new List<JobDto>();
        }
    }

    public async Task<bool> UpdateJobStatusAsync(int jobId, string status)
    {
        try
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                _logger.LogWarning("Job {JobId} not found", jobId);
                return false;
            }

            // ✅ FIX #6: Validate status transitions (State Machine)
            var currentStatus = job.Status;

            // Validate status value
            var validStatuses = new[] { "Pending", "Approved", "Rejected", "Closed", "Expired" };
            if (!validStatuses.Contains(status))
            {
                _logger.LogWarning("Invalid status: {Status}", status);
                return false;
            }

            // Define allowed transitions
            var allowedTransitions = new Dictionary<string, string[]>
            {
                ["Pending"] = new[] { "Approved", "Rejected" },
                ["Approved"] = new[] { "Closed", "Expired" },
                ["Rejected"] = Array.Empty<string>(), // Terminal state
                ["Closed"] = Array.Empty<string>(),   // Terminal state
                ["Expired"] = Array.Empty<string>()   // Terminal state
            };

            // Check if transition is allowed
            if (!allowedTransitions.ContainsKey(currentStatus))
            {
                _logger.LogWarning("Unknown current status: {CurrentStatus}", currentStatus);
                return false;
            }

            if (!allowedTransitions[currentStatus].Contains(status))
            {
                _logger.LogWarning("Invalid status transition: {CurrentStatus} -> {NewStatus}", currentStatus, status);
                return false;
            }

            job.Status = status;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Job status updated: {JobId} - {CurrentStatus} -> {NewStatus}", jobId, currentStatus, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job status for job {JobId}", jobId);
            return false;
        }
    }

    /// <summary>
    /// ✅ FIX #14: Increment view count with unique tracking
    /// Only count if user hasn't viewed in last 24 hours
    /// </summary>
    public async Task<bool> IncrementViewCountAsync(int jobId, int? userId = null, string? ipAddress = null, string? userAgent = null)
    {
        try
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return false;
            }

            var cutoffTime = DateTime.UtcNow.AddHours(-24);

            // Check if this user/IP already viewed in last 24h
            bool alreadyViewed;

            if (userId.HasValue)
            {
                // Logged-in user: Check by UserId
                alreadyViewed = await _context.JobViews
                    .AnyAsync(jv => jv.JobId == jobId &&
                                   jv.UserId == userId.Value &&
                                   jv.ViewedAt > cutoffTime);
            }
            else if (!string.IsNullOrEmpty(ipAddress))
            {
                // Anonymous user: Check by IP
                alreadyViewed = await _context.JobViews
                    .AnyAsync(jv => jv.JobId == jobId &&
                                   jv.IpAddress == ipAddress &&
                                   jv.ViewedAt > cutoffTime);
            }
            else
            {
                // No tracking info: Don't count
                _logger.LogWarning("IncrementViewCount called without userId or ipAddress for job {JobId}", jobId);
                return false;
            }

            if (!alreadyViewed)
            {
                // Record new view
                var jobView = new JobView
                {
                    JobId = jobId,
                    UserId = userId,
                    IpAddress = ipAddress,
                    UserAgent = userAgent,
                    ViewedAt = DateTime.UtcNow
                };

                await _context.JobViews.AddAsync(jobView);

                // Increment counter
                job.ViewCount++;

                await _context.SaveChangesAsync();

                _logger.LogInformation("Counted new view for job {JobId} (UserId: {UserId}, IP: {IP})",
                    jobId, userId, ipAddress?.Substring(0, Math.Min(ipAddress.Length, 10)));

                return true;
            }
            else
            {
                _logger.LogDebug("Skip counting duplicate view for job {JobId} (UserId: {UserId}, IP: {IP})",
                    jobId, userId, ipAddress?.Substring(0, Math.Min(ipAddress.Length, 10)));

                return false;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing view count for job {JobId}", jobId);
            return false;
        }
    }

    /// <summary>
    /// ✅ FIX #2: Batch expire all jobs past ApplicationDeadline
    /// Call this periodically (e.g., daily) or when needed
    /// </summary>
    public async Task<int> ExpireJobsPastDeadlineAsync()
    {
        try
        {
            var now = DateTime.UtcNow;

            var jobsToExpire = await _context.Jobs
                .Where(j => j.IsActive &&
                           j.Status == "Approved" &&
                           j.ApplicationDeadline.HasValue &&
                           j.ApplicationDeadline.Value < now)
                .ToListAsync();

            if (jobsToExpire.Count == 0)
            {
                _logger.LogInformation("No jobs to expire");
                return 0;
            }

            foreach (var job in jobsToExpire)
            {
                job.Status = "Expired";
                _logger.LogInformation("Expired job {JobId} '{Title}' (Deadline: {Deadline})",
                    job.Id, job.Title, job.ApplicationDeadline);
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Expired {Count} jobs past deadline", jobsToExpire.Count);
            return jobsToExpire.Count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error expiring jobs past deadline");
            return 0;
        }
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesForAdminAsync()
    {
        return await _context.Categories
            .OrderBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<Category?> GetCategoryByIdAsync(int id)
    {
        return await _context.Categories.FindAsync(id);
    }

    public async Task<Category> CreateCategoryAsync(string name, string? description)
    {
        try
        {
            var category = new Category
            {
                Name = name,
                Description = description,
                IsActive = true
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category created: {CategoryName}", name);
            return category;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating category {CategoryName}", name);
            throw;
        }
    }

    public async Task<bool> UpdateCategoryAsync(int id, string name, string? description, bool isActive)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            category.Name = name;
            category.Description = description;
            category.IsActive = isActive;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Category updated: {CategoryId}", id);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating category {CategoryId}", id);
            return false;
        }
    }

    public async Task<bool> ToggleCategoryStatusAsync(int id, bool isActive)
    {
        try
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return false;
            }

            category.IsActive = isActive;
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category status toggled: {CategoryId} - Active: {IsActive}", id, isActive);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error toggling category status {CategoryId}", id);
            return false;
        }
    }

    /// <summary>
    /// ✅ FIX #8: Delete category with validation
    /// Chỉ cho phép xóa nếu không có jobs đang active
    /// </summary>
    public async Task<bool> DeleteCategoryAsync(int id)
    {
        try
        {
            var category = await _context.Categories
                .Include(c => c.Jobs)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                _logger.LogWarning("Category {CategoryId} not found", id);
                return false;
            }

            // Check if category has active jobs
            var activeJobsCount = category.Jobs.Count(j => j.IsActive);
            if (activeJobsCount > 0)
            {
                _logger.LogWarning("Cannot delete category {CategoryId}: has {Count} active jobs", id, activeJobsCount);
                throw new InvalidOperationException($"Không thể xóa danh mục này vì còn {activeJobsCount} tin tuyển dụng đang hoạt động. Vui lòng vô hiệu hóa hoặc chuyển các tin sang danh mục khác trước.");
            }

            // Safe to delete
            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Category deleted successfully: {CategoryId} - {CategoryName}", id, category.Name);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting category {CategoryId}", id);
            throw;
        }
    }
}
