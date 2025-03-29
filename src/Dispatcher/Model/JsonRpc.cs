using System.Text.Json.Serialization;

namespace Dispatcher.Model;

public class JsonRpcRequest<TRequest>
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("method")]
    public string Method { get; set; }

    [JsonPropertyName("params")]
    public TRequest Params { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class JsonRpcSuccessResponse<TResponse>
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("result")]
    public TResponse Result { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class JsonRpcErrorResponse
{
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    [JsonPropertyName("error")]
    public JsonRpcError Error { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}

public class JsonRpcError
{
    [JsonPropertyName("code")]
    public int Code { get; set; }

    [JsonPropertyName("message")]
    public string Message { get; set; }
}
