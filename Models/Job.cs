using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobExchangeMvc.Models;

/// <summary>
/// Model tin tuyển dụng
/// </summary>
public class Job
{
    [Key]
    public int Id { get; set; }

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
    public string? JobType { get; set; } // Full-time, Part-time, Remote, Contract

    public int? NumberOfPositions { get; set; }

    public DateTime? ApplicationDeadline { get; set; }

    /// <summary>
    /// Trạng thái: Pending, Approved, Rejected, Closed
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    public int ViewCount { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // Foreign Keys
    public int CompanyId { get; set; }

    [ForeignKey(nameof(CompanyId))]
    public virtual Company Company { get; set; } = null!;

    public int CategoryId { get; set; }

    [ForeignKey(nameof(CategoryId))]
    public virtual Category Category { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
}
