namespace OrderManagementAPI.Response;

public class Page<T>(IEnumerable<T> content, int pageNumber, int pageSize, int totalElements)
{
    public IEnumerable<T> Content { get; set; } = content;
    public int PageNumber { get; set; } = pageNumber; // current page (0-based like Spring)
    public int PageSize { get; set; } = pageSize;
    public int TotalElements { get; set; } = totalElements;
    public int TotalPages => (int)Math.Ceiling((double)TotalElements / PageSize);
    public bool IsFirst => PageNumber == 0;
    public bool IsLast => PageNumber >= TotalPages - 1;
    
    public MetaData GetMetaData()
    {
        return new MetaData(TotalPages, PageNumber, TotalElements, PageSize );
    }

    /// <summary>
    /// Map the content of the page to another type while keeping pagination info
    /// </summary>
    public Page<U> MapContent<U>(Func<T, U> mapper)
    {
        var mappedContent = Content.Select(mapper).ToList();
        return new Page<U>(mappedContent, PageNumber, PageSize, TotalElements);
    }
}
