using BookApi.Models;

namespace BookApi.Specifications;

public class BookCountSpecification
    : BaseSpecification<Book>
{
    public BookCountSpecification(
        string? search)
    {
        if (!string.IsNullOrWhiteSpace(search))
        {
            AddCriteria(
                b => b.Title.Contains(search));
        }
    }
}