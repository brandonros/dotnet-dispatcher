using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

/// <summary>
/// Strongly-typed JSON-RPC request
/// </summary>
public class JsonRpcRequest<T> : JsonRpcRequestBase where T : IJsonRpcParams
{
    [JsonPropertyName("params")]
    [DefaultValue(null)]
    public new T Params { get; set; }
}
