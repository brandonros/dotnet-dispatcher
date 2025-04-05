using System.ComponentModel;
using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Attributes;

namespace Common.Model.JsonRpc;

/// <summary>
/// Base class for JSON-RPC requests that enforces type safety with method parameters
/// </summary>
[JsonConverter(typeof(JsonRpcRequestConverter))]
public class JsonRpcRequestBase
{
    public JsonRpcRequestBase()
    {
    }

    [JsonPropertyName("jsonrpc")]
    [DefaultValue("2.0")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("method")]
    public JsonRpcMethod Method { get; set; }

    [JsonPropertyName("id")]
    [Guid]
    [DefaultValue("a81bc81b-dead-4e5d-abff-90865d1e13b1")]
    public string Id { get; set; }
    
    [JsonPropertyName("params")]
    public JsonElement? Params { get; set; }
}