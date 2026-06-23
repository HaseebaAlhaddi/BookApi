using System.ComponentModel.DataAnnotations;
namespace BookApi.DTOs;

public class RegisterDto
{
    [Required]
    public required string Name { get; set; }
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
    [Required]
    [MinLength(6)]
    public required string Password { get; set; }
}