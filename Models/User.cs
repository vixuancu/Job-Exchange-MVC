using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobExchangeMvc.Models;

/// <summary>
/// Model người dùng - hỗ trợ cả Applicant (ứng viên) và Employer (nhà tuyển dụng)
/// </summary>
public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    [MaxLength(100)]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MaxLength(255)]
    public string PasswordHash { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [MaxLength(20)]
    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; set; }

    [MaxLength(255)]
    public string? AvatarUrl { get; set; }

    [MaxLength(255)]
    public string? CvUrl { get; set; }

    [MaxLength(500)]
    public string? Skills { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    /// <summary>
    /// Role: Admin, Employer, Applicant
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "Applicant";

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// VerifyKey - Mã xác minh (10 ký tự, bắt đầu bằng số, được mã hóa khi lưu DB)
    /// </summary>
    [Required(ErrorMessage = "VerifyKey là bắt buộc")]
    [StringLength(255, ErrorMessage = "VerifyKey đã mã hóa không hợp lệ")]
    [Display(Name = "Mã xác minh")]
    public string? VerifyKey { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public virtual Company? Company { get; set; }
    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();
    public virtual ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
}
