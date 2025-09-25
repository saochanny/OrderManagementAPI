using System.Text.Json.Serialization;

namespace OrderManagementAPI.Response;

public class BodyResponse
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public object? Data { get; set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public MetaData? Meta { get; set; }

    public BodyResponse() { }

    public BodyResponse(object data, MetaData? meta = null)
    {
        Data = data;
        Meta = meta;
    }
}