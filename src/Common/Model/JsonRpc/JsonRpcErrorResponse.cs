using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

/// <summary>
/// JSON-RPC error response model
/// </summary>
public class JsonRpcErrorResponse : JsonRpcResponse<object>
{
    // Add parameterless constructor
    public JsonRpcErrorResponse() : base()
    {
    }

    [JsonPropertyName("error")]
    public JsonRpcError Error { get; set; }
}
