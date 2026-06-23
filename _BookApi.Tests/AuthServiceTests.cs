using BookApi.DTOs;
using BookApi.Models;
using BookApi.Repositories;
using BookApi.Services;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;
namespace BookApi.Tests;
public class AuthServiceTests
{
    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenUserNotFound()
    {
        // Arrange
        var loginDto = new LoginDto
        {
             Email = "test@test.com",
            Password = "123456"
        };
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock
            .Setup(r => r.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User)null!);
        var configurationMock = new Mock<IConfiguration>();
        var authService = new AuthService(userRepositoryMock.Object, configurationMock.Object);
        // Act
        await Assert.ThrowsAsync<Exception>(() => authService.LoginAsync(loginDto));

    }
    [Fact]
    public async Task LoginAsync_ShouldThrowException_WhenPasswordIsWrong()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@test.com",
            Password = "123456"
        };
         var user = new User
    {
        Id = 1,
        Name = "Ali",
        Email = "test@test.com",

        // كلمة المرور الصحيحة
        PasswordHash =
            BCrypt.Net.BCrypt.HashPassword(
                "CorrectPassword"),

        Role = "User"
         };
        var userRepositoryMock = new Mock<IUserRepository>();
         userRepositoryMock
        .Setup(r => r.GetByEmailAsync(loginDto.Email))
        .ReturnsAsync(user);

    var configurationMock =
        new Mock<IConfiguration>();

    var authService =
        new AuthService(
            userRepositoryMock.Object,
            configurationMock.Object);

    // Act + Assert

    await Assert.ThrowsAsync<Exception>(
        () => authService.LoginAsync(loginDto));


    }
    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsAreValid()
    {
        // Arrange
         var loginDto = new LoginDto
    {
        Email = "test@test.com",
        Password = "123456"
    };

    var user = new User
    {
        Id = 1,
        Name = "Ali",
        Email = "test@test.com",
        PasswordHash =
            BCrypt.Net.BCrypt.HashPassword("123456"),
        Role = "User"
    };
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmailAsync(loginDto.Email)).ReturnsAsync(user);
        var configurationMock = new Mock<IConfiguration>();
        configurationMock
        .Setup(c => c["Jwt:Key"])
        .Returns("ThisIsMyVeryStrongSecretKeyForJwtAuthentication2025");

    configurationMock
        .Setup(c => c["Jwt:Issuer"])
        .Returns("BookApi");

    configurationMock
        .Setup(c => c["Jwt:Audience"])
        .Returns("BookApiUsers");
    var authService = new AuthService(userRepositoryMock.Object, configurationMock.Object);

    // Act
    var result = await authService.LoginAsync(loginDto);

    // Assert
    Assert.NotNull(result);
    Assert.False(string.IsNullOrEmpty(result.AccessToken));
    Assert.False(string.IsNullOrEmpty(result.RefreshToken));
    }
    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenEmailDoesNotExist()
    {
        // Arrange
        var registerDto = new RegisterDto
    {
        Name = "Ali",
        Email = "ali@test.com",
        Password = "123456"
    };

        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmailAsync(registerDto.Email)).ReturnsAsync((User?)null);
        var configurationMock = new Mock<IConfiguration>();
        var authService = new AuthService(userRepositoryMock.Object, configurationMock.Object);
        // Act
        var result = await authService.RegisterAsync(registerDto);
        // Assert
        Assert.NotNull(result);
        Assert.Equal(registerDto.Name, result.Name);
        Assert.Equal(registerDto.Email, result.Email);
       Assert.NotEqual(
    registerDto.Password,BCrypt.Net.BCrypt.HashPassword(result.PasswordHash));
        Assert.Equal("User", result.Role);
        userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
        userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
    [Fact]
    public async Task RegisterAsync_ShouldThrowException_WhenUserAlreadyExists()
    {
        // Arrange
        var registerDto = new RegisterDto
        {
            Name = "Ali",
            Email = "ali@test.com",
            Password = "123456"
        };
       var existingUser = new User
    {
        Id = 1,
        Name = "Ali",
        Email = "ali@test.com",
        PasswordHash = "hashedPassword",
        Role = "User"
       };
        var userRepositoryMock = new Mock<IUserRepository>();
        userRepositoryMock.Setup(r => r.GetByEmailAsync(registerDto.Email)).ReturnsAsync(existingUser);
        var configurationMock = new Mock<IConfiguration>();
        var authService = new AuthService(userRepositoryMock.Object, configurationMock.Object);
        // Act + Assert
        await Assert.ThrowsAsync<Exception>(() => authService.RegisterAsync(registerDto));
        userRepositoryMock.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
        userRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

}