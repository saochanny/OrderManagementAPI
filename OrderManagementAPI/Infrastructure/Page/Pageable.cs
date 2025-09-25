namespace OrderManagementAPI.Infrastructure.Page;

public class Pageable
{
    public int PageNumber { get; set; } = 0;  // 0-based index like Spring
    public int PageSize { get; set; } = 10;
    public string? SortBy { get; set; }
    public bool Ascending { get; set; } = true;

    public Pageable() {}

    public Pageable(int pageNumber, int pageSize = 10, string? sortBy = null, bool ascending = true)
    {
        PageNumber = pageNumber < 0 ? 0 : pageNumber;
        PageSize = pageSize <= 0 ? 10 : pageSize;
        SortBy = sortBy;
        Ascending = ascending;
    }
}
