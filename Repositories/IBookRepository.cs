using BookApi.Models;
using BookApi.Specifications;
using System.Collections.Generic;

namespace BookApi.Repositories;

public interface IBookRepository
{
    IQueryable<Book> GetQueryable();

    Task<Book?> GetByIdAsync(int id);

    Task AddAsync(Book book);

    Task UpdateAsync(Book book);

    Task DeleteAsync(Book book);
    Task <List<Book>> GetBooksWithSpecification(BaseSpecification<Book> specification);
    Task<int> CountAsync( BaseSpecification<Book> specification);
    Task SaveChangesAsync();
}