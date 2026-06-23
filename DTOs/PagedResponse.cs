namespace BookApi.DTOs;

public class PagedResponse<T>
{
    public int TotalCount { get; set; }

    public int Page { get; set; }

    public int PageSize { get; set; }

    public List<T> Data { get; set; } = [];
}