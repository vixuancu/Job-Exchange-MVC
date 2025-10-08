using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace JobExchangeMvc.Models;

/// <summary>
/// ✅ FIX #14: Model lưu lịch sử xem job (unique tracking)
/// </summary>
public class JobView
{
    [Key]
    public int Id { get; set; }

    [Required]
    public int JobId { get; set; }

    /// <summary>
    /// UserId nếu user đã login, null nếu anonymous
    /// </summary>
    public int? UserId { get; set; }

    /// <summary>
    /// IP Address để track anonymous users
    /// </summary>
    [MaxLength(45)] // IPv6 max length
    public string? IpAddress { get; set; }

    /// <summary>
    /// User Agent để phân biệt device
    /// </summary>
    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public DateTime ViewedAt { get; set; } = DateTime.UtcNow;

    // Navigation properties
    [ForeignKey("JobId")]
    public virtual Job Job { get; set; } = null!;

    [ForeignKey("UserId")]
    public virtual User? User { get; set; }
}
