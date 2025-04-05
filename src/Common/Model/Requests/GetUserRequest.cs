using System.Text.Json.Serialization;
using Common.Model.JsonRpc;

namespace Common.Model.Requests;

/// <summary>
/// Parameters for the GetUser method
/// </summary>
public class GetUserRequest : IJsonRpcParams
{
    [JsonPropertyName("userId")]
    public string UserId { get; set; }
}
