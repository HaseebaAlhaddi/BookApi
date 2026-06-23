using BookApi.Data;
using BookApi.Models;
using BookApi.Repositories;
using BookApi.Specifications;
using Microsoft.EntityFrameworkCore;
namespace BookApi.Repositories;
public class BookRepository : IBookRepository
{
    private readonly AppDbContext _context;
    public BookRepository(AppDbContext context)
    {
        _context = context;
    }
    public IQueryable<Book> GetQueryable()
{
    return _context.Books.AsQueryable();
}
    public async Task<Book?> GetByIdAsync(int id)
    {
         return await _context.Books
            .Include(b => b.Category)
            .FirstOrDefaultAsync(b => b.Id == id);
    }
    public async Task AddAsync(Book book)
    {
         await _context.Books.AddAsync(book);
    }
     public async Task UpdateAsync(Book book)
    {
        _context.Books.Update(book);

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Book book)
    {
        _context.Books.Remove(book);

        await Task.CompletedTask;
    }
    public async Task<List<Book>> GetBooksWithSpecification(BaseSpecification<Book> specification)
    {
        IQueryable<Book> query = _context.Books;
        foreach (var include in specification.Includes)
        
        {
            query = query.Include(include);
        }
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }
        if (specification.OrderBy != null)
        {
            query = query.OrderBy(specification.OrderBy);
        }
        if (specification.OrderByDescending != null)
        {
            query = query.OrderByDescending(specification.OrderByDescending);
        }
        if (specification.IsPagingEnabled)
    {
        query = query
            .Skip(specification.Skip)
            .Take(specification.Take);
    }

        return await query.ToListAsync();
    }
    public async Task<int> CountAsync(BaseSpecification<Book> specification)
    {
        IQueryable<Book> query = _context.Books;
        foreach (var include in specification.Includes)
        {
            query = query.Include(include);
        }
        if (specification.Criteria != null)
        {
            query = query.Where(specification.Criteria);
        }
        return await query.CountAsync();
    }
    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }

}