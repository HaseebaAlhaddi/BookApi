using System.Text.Json;
using BookApi.Exceptions;
using Serilog;
using BookApi.Responses;
namespace BookApi.Middleware;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    public ExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
       catch(Exception ex)
{
    Log.Error($"An error occurred: {ex.Message}");
    context.Response.ContentType = "application/json";

    context.Response.StatusCode =
        ex switch
        {
             NotFoundException => StatusCodes.Status404NotFound,

        UnauthorizedException => StatusCodes.Status401Unauthorized,

        BadRequestException => StatusCodes.Status400BadRequest,

        _ => StatusCodes.Status500InternalServerError
        };

    var response = new ApiResponse<string>
    {
        Success = false,
        Message = ex.Message,
        Data = null
    };

    var jsonResponse =
        JsonSerializer.Serialize(response);

    await context.Response.WriteAsync(jsonResponse);
}
    }
}