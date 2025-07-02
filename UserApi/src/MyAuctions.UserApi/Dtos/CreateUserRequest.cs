using System.ComponentModel.DataAnnotations;

namespace UserApi.Dtos;

public class CreateUserRequest
{
    [Required]
    public string Username { get; set; } = null!;

    [Required]
    public string Password { get; set; } = null!;
    
    [Required]
    public UserRole Role { get; set; }
}