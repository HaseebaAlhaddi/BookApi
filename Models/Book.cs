using System.ComponentModel.DataAnnotations.Schema;
namespace BookApi.Models;

public class Book
{
    public int Id { get; set; }
    public required string Title { get; set; }
    public required     string Author { get; set; }
    public required decimal Price { get; set; }
    public  string? ImageUrl { get; set; }
   public int CategoryId { get; set; }
   public Category? Category { get; set; }
}