using BookApi.Models;
namespace BookApi.Repositories;
public interface IUserRepository
{
    Task<User?> GetByIdAsync(int id);
    Task<User?> GetByEmailAsync(string email);
    Task AddAsync(User user);
    void Update(User user);
    void Delete(User user);
    Task SaveChangesAsync();
    Task<User?> GetByRefreshTokenAsync(string refreshToken);
}