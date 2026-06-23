using BookApi.Models;
namespace BookApi.Specifications;
public class BookSpecification : BaseSpecification<Book>
{
    public BookSpecification(string? search, string? sortBy, int page, int pageSize)
    {
       AddInclude(b => b.Category);
        if (!string.IsNullOrWhiteSpace(search))
        {
            AddCriteria(b => b.Title.Contains(search) || b.Author.Contains(search));
        }
        switch (sortBy?.ToLower())
        {
            case "title":
                AddOrderBy(b => b.Title);
                break;

            case "title_desc":
                AddOrderByDescending(b => b.Title);
                break;

            case "price":
                AddOrderBy(b => b.Price);
                break;

            case "price_desc":
                AddOrderByDescending(b => b.Price);
                break;

            default:
                AddOrderBy(b => b.Id);
                break;
        }
         ApplyPaging(
        (page - 1) * pageSize,
        pageSize);
    }
}