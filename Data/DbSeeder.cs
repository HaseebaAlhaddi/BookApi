using BookApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Data;
public static class DbSeeder
{
    public static async Task SeedAdminAsync(AppDbContext context)
    {
        if (await context.Users.AnyAsync((u => u.Email == "admin@bookapi.com")))
            return;
        var admin = new User
        {
            Name = "Admin",
            Email = "admin@bookapi.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        };
        await context.Users.AddAsync(admin);
        await context.SaveChangesAsync();

    }

}