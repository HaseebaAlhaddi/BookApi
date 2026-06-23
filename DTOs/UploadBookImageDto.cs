using Microsoft.AspNetCore.Http;

namespace BookApi.DTOs;

public class UploadBookImageDto
{
    public IFormFile Image { get; set; } = null!;
}