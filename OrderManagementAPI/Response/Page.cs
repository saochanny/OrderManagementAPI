namespace OrderManagementAPI.Response;

public class Page<T>
{
    public IEnumerable<T> Content { get; set; } = [];
    public int PageNumber { get; set; }     // current page (0-based like Spring)
    public int PageSize { get; set; }
    public int TotalElements { get; set; }
    public int TotalPages => (int)Math.Ceiling((double)TotalElements / PageSize);
    public bool IsFirst => PageNumber == 0;
    public bool IsLast => PageNumber >= TotalPages - 1;

    public Page() {}

    public MetaData GetMetaData()
    {
        return new MetaData(TotalPages, PageNumber, TotalElements, PageSize );
    }

    public Page(IEnumerable<T> content, int pageNumber, int pageSize, int totalElements)
    {
        Content = content;
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalElements = totalElements;
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
