using System.Text.Json.Serialization;

namespace Dispatcher.Model.Requests;

/// <summary>
/// Parameters for the ping method
/// </summary>
public class GetUserResponse
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("email")]
    public string Email { get; set; }
}
