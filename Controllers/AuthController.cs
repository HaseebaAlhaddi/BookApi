using Microsoft.AspNetCore.Mvc;
using BookApi.Services;
using BookApi.DTOs;

namespace BookApi.Controllers;
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthService _authService;
    public AuthController(AuthService authService)
    {
        _authService = authService;
    }
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto registerDto)
    {
        await _authService.RegisterAsync(registerDto);
        return Ok(new { message = "User registered successfully" });
    }
    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginDto loginDto)
    {
        var authResponse = await _authService.LoginAsync(loginDto);

        return Ok(authResponse);
    }
    [HttpPost("refresh")]
    public async Task<IActionResult> Refresh(RefreshTokenRequestDto request)
    {
        var result =
        await _authService.RefreshTokenAsync(request);

        return Ok(result);

    }
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenRequestDto request)
    {
        await _authService.LogoutAsync(request);
        return Ok(new { message = "Logged out successfully" });
    }
}