namespace BookApi.DTOs;

public class BookResponseDto
{
    public required int Id { get; set; }

    public required string Title { get; set; }

    public required string Author { get; set; }

    public required decimal Price { get; set; }
    public string? ImageUrl { get; set; }

    public required string CategoryName { get; set; }
}