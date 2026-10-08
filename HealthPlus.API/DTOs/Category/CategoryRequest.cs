using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.Category;

public class CategoryRequest
{
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public string? Description { get; set; }
}