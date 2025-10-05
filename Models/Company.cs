using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobExchangeMvc.Models;

/// <summary>
/// Model công ty - liên kết với User có Role = Employer
/// </summary>
public class Company
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên công ty là bắt buộc")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    [MaxLength(255)]
    public string? LogoUrl { get; set; }

    [MaxLength(200)]
    public string? Website { get; set; }

    [MaxLength(200)]
    public string? Address { get; set; }

    [MaxLength(100)]
    public string? City { get; set; }

    public int EmployerId { get; set; }

    [ForeignKey(nameof(EmployerId))]
    public virtual User Employer { get; set; } = null!;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
