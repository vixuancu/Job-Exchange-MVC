using System.ComponentModel.DataAnnotations;

namespace JobExchangeMvc.DTOs;

public class ProfileDto
{
    public int Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; set; }

    public string? AvatarUrl { get; set; }
    public string? CvUrl { get; set; }

    [MaxLength(500)]
    public string? Skills { get; set; }

    [MaxLength(1000)]
    public string? Bio { get; set; }

    // VerifyKey (encrypted in DB, shown as encrypted or decrypted)
    public string? VerifyKey { get; set; }

    // For password change
    public string? CurrentPassword { get; set; }

    [MinLength(6, ErrorMessage = "Mật khẩu mới phải có ít nhất 6 ký tự")]
    public string? NewPassword { get; set; }

    // For Employer
    public CompanyDto? Company { get; set; }

    // Flat properties for display (Admin views)
    public string? CompanyName { get; set; }
    public string? CompanyLogoUrl { get; set; }
}

public class CompanyDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Tên công ty là bắt buộc")]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? Description { get; set; }

    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
}
