using System.Text.Json.Serialization;

namespace Dispatcher.Model.Requests;

/// <summary>
/// Parameters for the ping method
/// </summary>
public class GetUserRequest
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
}
