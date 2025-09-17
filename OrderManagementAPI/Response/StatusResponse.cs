using System.Text.Json.Serialization;

namespace OrderManagementAPI.Response;

public class StatusResponse(int code, string message)
{
    [JsonPropertyName("code")]
    [System.ComponentModel.DataAnnotations.Schema.Column("code")]
    public int Code { get; set; } = code;

    [JsonPropertyName("message")]
    [System.ComponentModel.DataAnnotations.Schema.Column("message")]
    public string Message { get; set; } = !string.IsNullOrWhiteSpace(message) ? message : "Failed";
}