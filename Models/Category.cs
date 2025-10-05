using System.ComponentModel.DataAnnotations;

namespace JobExchangeMvc.Models;

/// <summary>
/// Danh mục ngành nghề
/// </summary>
public class Category
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Tên danh mục là bắt buộc")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation Properties
    public virtual ICollection<Job> Jobs { get; set; } = new List<Job>();
}
