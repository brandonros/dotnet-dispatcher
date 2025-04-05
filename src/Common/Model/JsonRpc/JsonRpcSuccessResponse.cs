using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

/// <summary>
/// JSON-RPC success response model
/// </summary>
public class JsonRpcSuccessResponse<TResponse> : JsonRpcResponse<TResponse>
{
    // Add parameterless constructor
    public JsonRpcSuccessResponse() : base()
    {
    }

    [JsonPropertyName("result")]
    public TResponse Result { get; set; }
}
