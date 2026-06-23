using AutoMapper;
using BookApi.DTOs;
using BookApi.Exceptions;
using BookApi.Models;
using BookApi.Repositories;
using BookApi.Specifications;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;

namespace BookApi.Services;

public class BookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IMapper _mapper;
    private readonly IMemoryCache _memoryCache;
    public BookService(IBookRepository bookRepository, IMapper mapper, IMemoryCache memoryCache)
    {
        _bookRepository = bookRepository;
        _mapper = mapper;
        _memoryCache = memoryCache;
    }

    public async Task<PagedResponse<BookResponseDto>> GetBooksAsync(
    string? search,
    string? sortBy,
    int page,
    int pageSize)
    {
        var cacheKey =
            $"books-{search}-{sortBy}-{page}-{pageSize}";

        if (_memoryCache.TryGetValue(
            cacheKey,
            out PagedResponse<BookResponseDto>? cachedResponse))
        {
            return cachedResponse!;
        }

        var specification =
            new BookSpecification(
                search,
                sortBy,
                page,
                pageSize);

        var books =
            await _bookRepository
                .GetBooksWithSpecification(specification);

        var countSpecification =
            new BookCountSpecification(search);

        var totalCount =
            await _bookRepository
                .CountAsync(countSpecification);

        var result =
            _mapper.Map<List<BookResponseDto>>(books);

        var response =
            new PagedResponse<BookResponseDto>
            {
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize,
                Data = result
            };

        _memoryCache.Set(
            cacheKey,
            response,
            TimeSpan.FromMinutes(5));

        return response;
    }
    public async Task<BookResponseDto?> GetBookByIdAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);

        if (book == null)
        {
            throw new NotFoundException("Book not found");
        }

        return _mapper.Map<BookResponseDto>(book);
    }

    public async Task<Book> CreateBookAsync(CreateBookDto createBookDto)
    {
        var book = _mapper.Map<Book>(createBookDto);

        await _bookRepository.AddAsync(book);
        await _bookRepository.SaveChangesAsync();

        _memoryCache.Remove("books");
        Log.Information("Book {BookId} created successfully", book.Id);

        return book;
    }

    public async Task<bool> UpdateBookAsync(int id, UpdateBookDto updateBookDto)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return false;
        }

        _mapper.Map(updateBookDto, book);

        await _bookRepository.UpdateAsync(book);
        await _bookRepository.SaveChangesAsync();
        _memoryCache.Remove("books");
        Log.Information("Book {BookId} updated successfully", book.Id);

        return true;
    }

    public async Task<bool> DeleteBookAsync(int id)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            return false;
        }

        await _bookRepository.DeleteAsync(book);
        await _bookRepository.SaveChangesAsync();
        _memoryCache.Remove("books");
        Log.Information("Book {BookId} deleted successfully", book.Id);
        return true;
    }
    public async Task<string> UploadBookImageAsync(int id, IFormFile image)
    {
        var book = await _bookRepository.GetByIdAsync(id);
        if (book == null)
        {
            throw new NotFoundException("Book not found");
        }
        if (image == null || image.Length == 0)
        {
            throw new BadRequestException("No image uploaded");
        }
        if (!string.IsNullOrEmpty(book.ImageUrl))
{
    var oldFilePath =
        Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot",
            book.ImageUrl.TrimStart('/'));

    if (File.Exists(oldFilePath))
    {
        File.Delete(oldFilePath);
    }
}
        var uploadFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        if (!Directory.Exists(uploadFolder))
        {
            Directory.CreateDirectory(uploadFolder);
        }
        var allowedExtensions =
    new[] { ".jpg", ".jpeg", ".png", ".webp" };

var extension =
    Path.GetExtension(image.FileName)
        .ToLower();

        if (!allowedExtensions.Contains(extension))
        {
            throw new BadRequestException(
                "Only image files are allowed");
        }
const long maxSize = 5 * 1024 * 1024;

if (image.Length > maxSize)
{
    throw new BadRequestException(
        "Image size must be less than 5 MB");
}
       var fileName =
    $"{Guid.NewGuid()}{Path.GetExtension(image.FileName)}";
        var filePath = Path.Combine(uploadFolder, fileName);
        using (var stream = new FileStream(filePath, FileMode.Create))
        {
            await image.CopyToAsync(stream);
        }
        book.ImageUrl = $"/uploads/{fileName}";
        await _bookRepository.UpdateAsync(book);
        await _bookRepository.SaveChangesAsync();
        return fileName;
        
    }
}
