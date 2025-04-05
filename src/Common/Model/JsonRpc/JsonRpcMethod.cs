using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

/// <summary>
/// Available JSON-RPC methods
/// </summary>
[JsonConverter(typeof(JsonRpcMethodConverter))]
public class JsonRpcMethod
{
    private string _method;

    public JsonRpcMethod(string method)
    {
        _method = method ?? throw new ArgumentNullException(nameof(method));
    }

    public static JsonRpcMethod GetUser => new("user.get");
    public static JsonRpcMethod GetAccount => new("account.get"); // Add GetAccount method

    public override string ToString() => _method;
    public override bool Equals(object obj) => obj is JsonRpcMethod other && other._method == _method;
    public override int GetHashCode() => _method.GetHashCode();

    public static implicit operator string(JsonRpcMethod method) => method.ToString();
    public static implicit operator JsonRpcMethod(string method) => new(method);
}
