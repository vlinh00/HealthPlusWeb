using System.ComponentModel.DataAnnotations;

namespace HealthPlus.API.DTOs.User;

public class UpdateUserStatusRequest
{
    [Required]
    public bool IsActive { get; set; }
}