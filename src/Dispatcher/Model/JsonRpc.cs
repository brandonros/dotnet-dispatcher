using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace Dispatcher.Model;

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
[JsonConverter(typeof(JsonStringEnumConverter))]
public enum JsonRpcMethod
{
    /// <summary>
    /// Ping method to check service availability
    /// </summary>
    [JsonPropertyName("ping")]
    Ping
}

/// <summary>
/// JSON-RPC request model
/// </summary>
public class JsonRpcRequest<TRequest>
{
    /// <summary>
    /// JSON-RPC protocol version
    /// </summary>
    /// <example>2.0</example>
    [JsonPropertyName("jsonrpc")]
    public string JsonRpc { get; set; } = "2.0";

    /// <summary>
    /// The method to be invoked
    /// </summary>
    /// <example>ping</example>
    [JsonPropertyName("method")]
    public JsonRpcMethod Method { get; set; }

    /// <summary>
    /// The parameters for the method
    /// </summary>
    [JsonPropertyName("params")]
    public TRequest Params { get; set; }

    /// <summary>
    /// The request identifier (must be a valid GUID/UUID)
    /// </summary>
    /// <example>123e4567-e89b-12d3-a456-426614174000</example>
    [JsonPropertyName("id")]
    [Guid]
    public string Id { get; set; }
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
