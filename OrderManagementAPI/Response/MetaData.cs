namespace OrderManagementAPI.Response;

public class MetaData
{
    public int TotalPage { get; set; }
    public int Page { get; set; }
    public long TotalCount { get; set; }
    public int PageSize { get; set; }
    public bool HasNext  => Page < TotalPage - 1;
    public bool HasPrevious  => Page > 0;
    
    public MetaData() { }
    
    public MetaData(int totalPage, int page, long totalCount, int pageSize)
    {
        TotalPage = totalPage;
        Page = page;
        TotalCount = totalCount;
        PageSize = pageSize;
    }
}