using System.ComponentModel.DataAnnotations;

namespace JobExchangeMvc.DTOs;

public class RegisterDto
{
    [Required(ErrorMessage = "Email là bắt buộc")]
    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
    [MinLength(6, ErrorMessage = "Mật khẩu phải có ít nhất 6 ký tự")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
    [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
    public string ConfirmPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(100)]
    public string FullName { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Vai trò là bắt buộc")]
    public string Role { get; set; } = "Applicant"; // Applicant or Employer

    [Required(ErrorMessage = "VerifyKey là bắt buộc")]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "VerifyKey phải có đúng 10 ký tự")]
    [RegularExpression(@"^\d.*", ErrorMessage = "VerifyKey phải bắt đầu bằng chữ số (0-9)")]
    [Display(Name = "Mã xác minh")]
    public string VerifyKey { get; set; } = string.Empty;
}
