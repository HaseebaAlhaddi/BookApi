using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using BookApi.Data;
using BookApi.DTOs;
using BookApi.Exceptions;
using BookApi.Models;
using BookApi.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace BookApi.Services;
public class AuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;
    public AuthService(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }
    private string GenerateToken(User user)
    {
        var claims = new[]{
            new Claim(ClaimTypes.Name, user.Name),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role),
        };
        var jwtKey = _configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "Jwt:Key is missing");

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwtKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(
            issuer: _configuration["Jwt:Issuer"],
            audience: _configuration["Jwt:Audience"],
            claims: claims,
            expires: DateTime.Now.AddHours(2),
            signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);

    }
    private string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        
        return Convert.ToBase64String(randomBytes);
    }


    public async Task<User> RegisterAsync(RegisterDto registerDto)
    {
        var existingUser = await _userRepository.GetByEmailAsync(registerDto.Email);
        if (existingUser != null)
        {
            throw new BadRequestException("User already exists");
        }
        var user = new User
        {
            Name = registerDto.Name,
            Email = registerDto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(registerDto.Password),
            Role = "User",
        };
        await _userRepository.AddAsync(user);
        await _userRepository.SaveChangesAsync();
        return user;
    }
    public async Task<AuthResponseDto> LoginAsync(LoginDto loginDto)
    {
        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null)
        {
            Log.Warning("User {Email} not found", loginDto.Email);
            throw new UnauthorizedException("Invalid email or password");
        }
        var isPasswordCorrect = BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash);
        if (!isPasswordCorrect)
        {
            Log.Warning("Invalid email or password for user {Email}", loginDto.Email);
            throw new UnauthorizedException("Invalid email or password");
        }
        Log.Information("User {Email} logged in successfully", user.Email);
        var accessToken = GenerateToken(user);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime =
    DateTime.UtcNow.AddDays(7);

        await _userRepository.SaveChangesAsync();
        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = user.RefreshToken };
    }
    public async Task<AuthResponseDto> RefreshTokenAsync(RefreshTokenRequestDto refreshTokenRequestDto)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(refreshTokenRequestDto.RefreshToken);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }
        if (user.RefreshTokenExpiryTime < DateTime.UtcNow)
        {
            throw new UnauthorizedException("Refresh token expired");
        }
        var accessToken = GenerateToken(user);
        user.RefreshToken = GenerateRefreshToken();
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userRepository.SaveChangesAsync();
        Log.Information("Refresh token used by {Email} successfully",user.Email);
        return new AuthResponseDto { AccessToken = accessToken, RefreshToken = user.RefreshToken };
    }
    public async Task LogoutAsync(RefreshTokenRequestDto request)
    {
        var user = await _userRepository.GetByRefreshTokenAsync(request.RefreshToken);
        if (user == null)
        {
            throw new UnauthorizedException("Invalid refresh token");
        }
        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = null;
        Log.Information("User {Email} logged out successfully",user.Email);
        await _userRepository.SaveChangesAsync();
    }
 }