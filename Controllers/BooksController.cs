using BookApi.Data;
using BookApi.Models;
using BookApi.Responses;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using BookApi.DTOs;
using BookApi.Services; //BookService
namespace BookApi.Controllers;
[Authorize]

[ApiController]
[Route("api/[controller]")]
public class BooksController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly BookService _bookService;
    public BooksController(AppDbContext context, BookService bookService)
    {
        _context = context;
        _bookService = bookService;
    }
    [EnableRateLimiting("fixed")]
    [HttpGet]
    public async Task<IActionResult> GetBooks(
     string? search,
     string? sortBy,
     int page = 1,
     int pageSize = 5)
    {
        var books = await _bookService.GetBooksAsync(
            search,
            sortBy,
            page,
            pageSize);

        return Ok(new ApiResponse<PagedResponse<BookResponseDto>>
        {
            Success = true,
            Message = "Books fetched successfully",
            Data = books
        });
    }
    [HttpGet("{id}")]
    public async Task<ActionResult<Book>> GetBook(int id)
    {
        var book = await _bookService.GetBookByIdAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        return Ok(new ApiResponse<BookResponseDto>
        {
            Success = true,
            Message = "Book fetched successfully",
            Data = book
        });
    }
    [Authorize(Roles = "Admin")]
    [HttpPost]
    public async Task<ActionResult<Book>> PostBook(CreateBookDto createBookDto)
    {
        var book = await _bookService.CreateBookAsync(createBookDto);
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }
    /*public async Task<ActionResult<Book>> PostBook(Book book)
    {
        _context.Books.Add(book);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }*/
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, UpdateBookDto updateBookDto)
    {
        var result = await _bookService.UpdateBookAsync(id, updateBookDto);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
    /*public async Task<IActionResult> Update(int id,Book updatedBook)
    {
        var book = await _context.Books.FindAsync(id);
        if (book == null)
        {
            return NotFound();
        }
        book.Title = updatedBook.Title;
        book.Author = updatedBook.Author;
        book.Price = updatedBook.Price;
        await _context.SaveChangesAsync();
        return NoContent();
    }*/
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _bookService.DeleteBookAsync(id);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
    [HttpPost("{id}/upload-image")]
[Authorize(Roles = "Admin")]
public async Task<IActionResult> UploadImage(
    int id,
    IFormFile image)
{
    var imageUrl =
        await _bookService
            .UploadBookImageAsync(
                id,
                image);

    return Ok(
        new ApiResponse<string>
        {
            Success = true,
            Message = "Image uploaded successfully",
            Data = imageUrl
        });
}

}
