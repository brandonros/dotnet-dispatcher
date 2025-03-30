using System.Text.Json.Serialization;

namespace Common.Model.Responses;

/// <summary>
/// Parameters for the GetUser method
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
