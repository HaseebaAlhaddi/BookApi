using System.ComponentModel.DataAnnotations;
namespace BookApi.DTOs;
public class CreateCategoryDto
{
    [Required]
    public required string Name { get; set; }
}