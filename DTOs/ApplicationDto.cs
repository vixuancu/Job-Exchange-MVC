using System.ComponentModel.DataAnnotations;

namespace JobExchangeMvc.DTOs;

public class ApplicationDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Job ID là bắt buộc")]
    public int JobId { get; set; }

    [MaxLength(1000)]
    public string? CoverLetter { get; set; }

    public string? CvUrl { get; set; }

    public string? Status { get; set; }
    public string? Note { get; set; }

    // Display info
    public string? JobTitle { get; set; }
    public string? CompanyName { get; set; }
    public string? ApplicantName { get; set; }
    public string? ApplicantEmail { get; set; }
    public DateTime? AppliedAt { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
