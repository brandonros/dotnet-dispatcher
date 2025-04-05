using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

/// <summary>
/// Base class for all JSON-RPC responses
/// </summary>
public abstract class JsonRpcResponse<TResponse>
{
    // Add parameterless constructor
    protected JsonRpcResponse()
    {
        JsonRpc = "2.0";
    }

    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}


