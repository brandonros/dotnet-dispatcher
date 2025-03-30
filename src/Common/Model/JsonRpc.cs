using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Common.Model.Requests;
using System.Text.Json;
using System.ComponentModel;

namespace Common.Model;

/// <summary>
/// Validates that a string is a valid GUID/UUID
/// </summary>
public class GuidAttribute : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        if (value == null) return ValidationResult.Success;
        
        if (value is string strValue)
        {
            if (Guid.TryParse(strValue, out _))
            {
                return ValidationResult.Success;
            }
        }
        
        return new ValidationResult("The field must be a valid GUID/UUID.");
    }
}

/// <summary>
/// Available JSON-RPC methods
/// </summary>
public class JsonRpcMethod
{
    private readonly string _method;
    
    public JsonRpcMethod(string method)
    {
        _method = method ?? throw new ArgumentNullException(nameof(method));
    }

    public static JsonRpcMethod GetUser => new("user.get");
    
    public override string ToString() => _method;
    
    public static implicit operator string(JsonRpcMethod method) => method.ToString();
    public static implicit operator JsonRpcMethod(string method) => new(method);
}

/// <summary>
/// Base interface for all JSON-RPC parameter types
/// </summary>
public interface IJsonRpcParams { }

/// <summary>
/// Base class for JSON-RPC requests that enforces type safety with method parameters
/// </summary>
public abstract class JsonRpcRequestBase
{
    protected JsonRpcRequestBase()
    {
        // Parameterless constructor for JSON deserialization
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
}

/// <summary>
/// Strongly-typed JSON-RPC request
/// </summary>
public class JsonRpcRequest<T> : JsonRpcRequestBase where T : IJsonRpcParams
{
    [JsonPropertyName("params")]
    [DefaultValue(null)]
    public T Params { get; set; }
}

/// <summary>
/// Method-specific request types for better type safety and discoverability
/// </summary>
public class GetUserJsonRpcRequest : JsonRpcRequest<GetUserRequest>
{
    public GetUserJsonRpcRequest()
    {
        Method = JsonRpcMethod.GetUser;
    }
}

/// <summary>
/// JSON-RPC success response model
/// </summary>
public class JsonRpcSuccessResponse<TResponse>
{
    /// <summary>
    /// JSON-RPC protocol version
    /// </summary>
    /// <example>2.0</example>
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    /// <summary>
    /// The result of the method invocation
    /// </summary>
    [JsonPropertyName("result")]
    public TResponse Result { get; set; }

    /// <summary>
    /// The request identifier
    /// </summary>
    /// <example>1</example>
    [JsonPropertyName("id")]
    public string Id { get; set; }
}

/// <summary>
/// JSON-RPC error response model
/// </summary>
public class JsonRpcErrorResponse
{
    /// <summary>
    /// JSON-RPC protocol version
    /// </summary>
    /// <example>2.0</example>
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    /// <summary>
    /// The error details
    /// </summary>
    [JsonPropertyName("error")]
    public JsonRpcError Error { get; set; }

    /// <summary>
    /// The request identifier
    /// </summary>
    /// <example>1</example>
    [JsonPropertyName("id")]
    public string Id { get; set; }
}

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
