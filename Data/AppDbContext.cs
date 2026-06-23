using BookApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BookApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {

    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<User> Users { get; set; }
    protected override void OnModelCreating(
    ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Book>()
        .Property(b => b.Price)
        .HasPrecision(18, 2);

    base.OnModelCreating(modelBuilder);
}
}