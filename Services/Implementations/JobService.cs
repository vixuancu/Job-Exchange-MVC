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

    public async Task<IEnumerable<JobDto>> GetAllJobsAsync(string? searchTerm = null, int? categoryId = null, string? location = null)
    {
        try
        {
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

            var jobs = await query
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
            _logger.LogError(ex, "Error getting jobs");
            return new List<JobDto>();
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
                ViewCount = job.ViewCount,
                CompanyId = job.CompanyId,
                CompanyName = job.Company.Name,
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
            // Get employer's company
            var company = await _context.Companies
                .FirstOrDefaultAsync(c => c.EmployerId == employerId);

            if (company == null)
            {
                throw new InvalidOperationException("Employer must have a company to create jobs");
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
            var job = await _context.Jobs
                .Include(j => j.Company)
                .FirstOrDefaultAsync(j => j.Id == jobId && j.Company.EmployerId == employerId);

            if (job == null)
            {
                return false;
            }

            // Soft delete
            job.IsActive = false;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Job deleted successfully: {JobId}", jobId);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting job {JobId}", jobId);
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
                .Where(j => j.Company.EmployerId == employerId)
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
            _logger.LogError(ex, "Error getting jobs for employer {EmployerId}", employerId);
            return new List<JobDto>();
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
                return false;
            }

            // Validate status
            if (status != "Pending" && status != "Approved" && status != "Rejected" && status != "Closed")
            {
                return false;
            }

            job.Status = status;
            job.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            _logger.LogInformation("Job status updated: {JobId} - Status: {Status}", jobId, status);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating job status for job {JobId}", jobId);
            return false;
        }
    }

    public async Task<bool> IncrementViewCountAsync(int jobId)
    {
        try
        {
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return false;
            }

            job.ViewCount++;
            await _context.SaveChangesAsync();
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error incrementing view count for job {JobId}", jobId);
            return false;
        }
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _context.Categories
            .Where(c => c.IsActive)
            .OrderBy(c => c.Name)
            .ToListAsync();
    }
}
