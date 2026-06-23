using AutoMapper;
using BookApi.DTOs;
using BookApi.Exceptions;
using BookApi.Models;
using BookApi.Repositories;
using BookApi.Services;
using Microsoft.Extensions.Caching.Memory;
using Moq;
using Xunit;

namespace BookApi.Tests;

public class BookServiceTests
{
    private static IMemoryCache CreateMemoryCache() =>
        new MemoryCache(new MemoryCacheOptions());

    [Fact]
    public async Task GetBookByIdAsync_ReturnsBook_WhenBookExists()
    {
        var book = new Book
        {
            Id = 1,
            Title = "Clean Code",
            Author = "Robert Martin",
            Price = 100
        };
        var bookDto = new BookResponseDto
        {
            Id = 1,
            Title = "Clean Code",
            Author = "Robert Martin",
            Price = 100,
            CategoryName = "Programming"
        };
        var repositoryMock = new Mock<IBookRepository>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync(book);

        var mapperMock = new Mock<IMapper>();

        mapperMock
            .Setup(m => m.Map<BookResponseDto>(book))
            .Returns(bookDto);

        var service = new BookService(
            repositoryMock.Object,
            mapperMock.Object,
            CreateMemoryCache());

        var result = await service.GetBookByIdAsync(1);

        Assert.NotNull(result);
        Assert.Equal(1, result.Id);
        Assert.Equal("Clean Code", result.Title);
    }
    [Fact]
    public async Task
GetBookByIdAsync_ShouldThrowException_WhenBookNotFound()
    {
        // Arrange

        var repositoryMock =
            new Mock<IBookRepository>();

        repositoryMock
            .Setup(r => r.GetByIdAsync(1))
            .ReturnsAsync((Book?)null);

        var mapperMock =
            new Mock<IMapper>();

        var service =
            new BookService(
                repositoryMock.Object,
                mapperMock.Object,
                CreateMemoryCache());

        // Act + Assert

        await Assert.ThrowsAsync<NotFoundException>(
            () => service.GetBookByIdAsync(1));
    }
[Fact]
public async Task CreateBookAsync_ShouldAddBook_WhenDataIsValid()
{
    // Arrange

    var createBookDto = new CreateBookDto
    {
        Title = "Clean Code",
        Author = "Robert Martin",
        Price = 100,
        CategoryId = 1
    };

    var book = new Book
    {
        Id = 1,
        Title = "Clean Code",
        Author = "Robert Martin",
        Price = 100,
        CategoryId = 1
    };

    var repositoryMock =
        new Mock<IBookRepository>();

    var mapperMock =
        new Mock<IMapper>();

    mapperMock
        .Setup(m => m.Map<Book>(createBookDto))
        .Returns(book);

    var service =
        new BookService(
            repositoryMock.Object,
            mapperMock.Object,
            CreateMemoryCache());

    // Act

    var result =
        await service.CreateBookAsync(createBookDto);

    // Assert

    Assert.NotNull(result);

    Assert.Equal(
        "Clean Code",
        result.Title);

    repositoryMock.Verify(
        r => r.AddAsync(It.IsAny<Book>()),
        Times.Once);

    repositoryMock.Verify(
        r => r.SaveChangesAsync(),
        Times.Once);
}
[Fact]
public async Task UpdateBookAsync_ShouldReturnTrue_WhenBookExists()
{
    // Arrange
    var book = new Book
    {
        Id = 1,
        Title = "Clean Code",
        Author = "Robert Martin",
        Price = 100,
        CategoryId = 1
    };
    var updateBookDto = new UpdateBookDto
    {
        Title = "New Title",
        Author = "New Author",
        Price = 200,
    };
    var repositoryMock = new Mock<IBookRepository>();
    var mapperMock = new Mock<IMapper>();
    repositoryMock.Setup(r=>r.GetByIdAsync(1)).ReturnsAsync(book);
    var service = new BookService(repositoryMock.Object, mapperMock.Object, CreateMemoryCache());
    var result = await service.UpdateBookAsync(1, updateBookDto);
    Assert.True(result);
    repositoryMock.Verify(r=>r.UpdateAsync(It.IsAny<Book>()), Times.Once);
    repositoryMock.Verify(r=>r.SaveChangesAsync(), Times.Once);
}
[Fact]
public async Task UpdateBookAsync_ShouldReturnFalse_WhenBookNotFound()
{
    // Arrange
    var updateBookDto = new UpdateBookDto
    {
        Title = "New Title",
        Author = "New Author",
        Price = 200,
    };
    var repositoryMock = new Mock<IBookRepository>();
    repositoryMock.Setup(r=>r.GetByIdAsync(1)).ReturnsAsync((Book?)null);
    var mapperMock = new Mock<IMapper>();
    var service = new BookService(repositoryMock.Object, mapperMock.Object, CreateMemoryCache());
    var result = await service.UpdateBookAsync(1, updateBookDto);
    Assert.False(result);
    repositoryMock.Verify(r=>r.UpdateAsync(It.IsAny<Book>()), Times.Never);
    repositoryMock.Verify(r=>r.SaveChangesAsync(), Times.Never);
}
[Fact]
public async Task DeleteBookAsync_ShouldDeleteBook_WhenBookExists()
{
    // Arrange
    var book = new Book
    {
        Id = 1,
        Title = "Clean Code",
        Author = "Robert Martin",
        Price = 100,
        CategoryId = 1
    };
    var repositoryMock = new Mock<IBookRepository>();
    repositoryMock.Setup(r=>r.GetByIdAsync(1)).ReturnsAsync(book);
    var mapperMock = new Mock<IMapper>();
    var service = new BookService(repositoryMock.Object, mapperMock.Object, CreateMemoryCache());
    // Act
    var result= await service.DeleteBookAsync(1);
    Assert.True(result);
    repositoryMock.Verify(r=>r.DeleteAsync(It.IsAny<Book>()), Times.Once);
    repositoryMock.Verify(r=>r.SaveChangesAsync(), Times.Once);
}
[Fact]
public async Task DeleteBookAsync_ShouldReturnFalse_WhenBookNotFound()
{
    
    var repositoryMock = new Mock<IBookRepository>();
    repositoryMock.Setup(r=>r.GetByIdAsync(1)).ReturnsAsync((Book?)null);
    var mapperMock = new Mock<IMapper>();
    var service = new BookService(repositoryMock.Object, mapperMock.Object, CreateMemoryCache());
    var result = await service.DeleteBookAsync(1);
    Assert.False(result);
    repositoryMock.Verify(r=>r.DeleteAsync(It.IsAny<Book>()), Times.Never);
    repositoryMock.Verify(r=>r.SaveChangesAsync(), Times.Never);
}
}
