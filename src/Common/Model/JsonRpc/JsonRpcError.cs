using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

/// <summary>
/// JSON-RPC error details
/// </summary>
public class JsonRpcError
{
    /// <summary>
    /// Error code
    /// </summary>
    /// <example>-32601</example>
    [JsonPropertyName("code")]
    public int Code { get; set; }

    /// <summary>
    /// Error message
    /// </summary>
    /// <example>Method not found</example>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
