using System.Text.Json.Serialization;

namespace OrderManagementAPI.Response;

public class BaseResponse
{
    public class BodyResponse
    {
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public object? Data { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public PageResponse? Page { get; set; }

        public BodyResponse() { }

        public BodyResponse(object data, PageResponse? page = null)
        {
            Data = data;
            Page = page;
        }
    }
}