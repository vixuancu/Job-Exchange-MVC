using System.ComponentModel.DataAnnotations;

namespace JobExchangeMvc.DTOs;

public class JobDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mô tả công việc là bắt buộc")]
    [MaxLength(5000)]
    public string Description { get; set; } = string.Empty;

    [MaxLength(1000)]
    public string? Requirements { get; set; }

    [MaxLength(1000)]
    public string? Benefits { get; set; }

    [MaxLength(100)]
    public string? SalaryRange { get; set; }

    [MaxLength(100)]
    public string? Location { get; set; }

    [MaxLength(50)]
    public string? JobType { get; set; }

    public int? NumberOfPositions { get; set; }

    public DateTime? ApplicationDeadline { get; set; }

    [Required(ErrorMessage = "Danh mục là bắt buộc")]
    public int CategoryId { get; set; }

    public int? CompanyId { get; set; }

    // Company info for display
    public string? CompanyName { get; set; }
    public string? CompanyLogoUrl { get; set; }
    public string? CompanyDescription { get; set; }
    public string? CompanyWebsite { get; set; }
    public string? CompanyAddress { get; set; }
    public string? CompanyCity { get; set; }
    public int? CompanyEmployerId { get; set; }
    
    // Category info
    public string? CategoryName { get; set; }
    
    // Job status info
    public string? Status { get; set; }
    public bool IsActive { get; set; }
    public int ViewCount { get; set; }
    public DateTime? CreatedAt { get; set; }
}
