using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobExchangeMvc.Models;

/// <summary>
/// Model đơn ứng tuyển
/// </summary>
public class Application
{
    [Key]
    public int Id { get; set; }

    [MaxLength(1000)]
    public string? CoverLetter { get; set; }

    [MaxLength(255)]
    public string? CvUrl { get; set; }

    /// <summary>
    /// Trạng thái: Pending, Accepted, Rejected, Cancelled
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Status { get; set; } = "Pending";

    [MaxLength(500)]
    public string? Note { get; set; } // Ghi chú từ nhà tuyển dụng

    // Foreign Keys
    public int JobId { get; set; }

    [ForeignKey(nameof(JobId))]
    public virtual Job Job { get; set; } = null!;

    public int ApplicantId { get; set; }

    [ForeignKey(nameof(ApplicantId))]
    public virtual User Applicant { get; set; } = null!;

    public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }
}
