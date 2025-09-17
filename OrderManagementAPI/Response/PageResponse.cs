namespace OrderManagementAPI.Response;

public class PageResponse
{
    public int TotalPage { get; set; }
    public int Page { get; set; }
    public long TotalCount { get; set; }
    public int PageSize { get; set; }

    public PageResponse() { }

    private PageResponse(int totalPage, int page, long totalCount, int pageSize)
    {
        TotalPage = totalPage;
        Page = page;
        TotalCount = totalCount;
        PageSize = pageSize;
    }

    // Factory method for LINQ or EF Core pagination
    public static PageResponse FromPagedResult<T>(IQueryable<T> query, int page, int pageSize)
    {
        var totalCount = query.LongCount();
        var totalPage = (int)Math.Ceiling(totalCount / (double)pageSize);

        return new PageResponse(totalPage, page, totalCount, pageSize);
    }
}