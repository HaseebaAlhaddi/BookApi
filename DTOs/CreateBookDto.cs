using System.ComponentModel.DataAnnotations;

namespace BookApi.DTOs;

public class CreateBookDto
{
    [Required]
    public required string Title { get; set; }
    [Required]
    public required string Author { get; set; }
    [Range(1, 1000)]
    public required decimal Price { get; set; }
    [Required]
  
    public required int CategoryId { get; set; }
}