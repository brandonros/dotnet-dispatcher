using System.Text.Json.Serialization;

namespace Common.Model.Requests;

/// <summary>
/// Parameters for the ping method
/// </summary>
public class GetUserRequest : IJsonRpcParams
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
}
