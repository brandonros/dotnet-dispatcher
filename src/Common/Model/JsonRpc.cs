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

public class JsonRpcMethodConverter : JsonConverter<JsonRpcMethod>
{
    public override JsonRpcMethod Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType == JsonTokenType.String)
        {
            return new JsonRpcMethod(reader.GetString());
        }
        else
        {
            throw new JsonException($"Unable to convert {reader.TokenType} to JsonRpcMethod.");
        }
    }

    public override void Write(Utf8JsonWriter writer, JsonRpcMethod value, JsonSerializerOptions options)
    {
        writer.WriteStringValue(value.ToString());
    }
}

/// <summary>
/// Base interface for all JSON-RPC parameter types
/// </summary>
public interface IJsonRpcParams { }

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

public class JsonRpcRequestConverter : JsonConverter<JsonRpcRequestBase>
{
    public override JsonRpcRequestBase Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException();
            
        using var jsonDoc = JsonDocument.ParseValue(ref reader);
        var root = jsonDoc.RootElement;
        var methodProp = root.GetProperty("method");
        var method = methodProp.GetString();
        
        // Create the appropriate request type based on the method
        JsonRpcRequestBase request = method switch
        {
            "user.get" => new GetUserJsonRpcRequest(),
            "account.get" => new GetAccountJsonRpcRequest(), // Add case for GetAccount
            _ => new JsonRpcRequestBase()
        };
        
        // Deserialize into the specific type
        return JsonSerializer.Deserialize(root.GetRawText(), request.GetType(), options) as JsonRpcRequestBase
            ?? throw new JsonException("Failed to deserialize request");
    }
    
    public override void Write(Utf8JsonWriter writer, JsonRpcRequestBase value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}


/// <summary>
/// Strongly-typed JSON-RPC request
/// </summary>
public class JsonRpcRequest<T> : JsonRpcRequestBase where T : IJsonRpcParams
{
    [JsonPropertyName("params")]
    [DefaultValue(null)]
    public new T Params { get; set; }
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
/// Add GetAccount request model
/// </summary>
public class GetAccountRequest : IJsonRpcParams
{
    [Required]
    [Guid]
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }
}

/// <summary>
/// Add GetAccount response model
/// </summary>
public class GetAccountResponse
{
    [JsonPropertyName("accountId")]
    public string AccountId { get; set; }
    
    [JsonPropertyName("accountName")]
    public string AccountName { get; set; }
    
    [JsonPropertyName("accountType")]
    public string AccountType { get; set; }
    
    [JsonPropertyName("balance")]
    public decimal Balance { get; set; }
    
    [JsonPropertyName("currencyCode")]
    public string CurrencyCode { get; set; }
    
    [JsonPropertyName("createdAt")]
    public DateTime CreatedAt { get; set; }
    
    [JsonPropertyName("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Add GetAccountJsonRpcRequest type
/// </summary>
public class GetAccountJsonRpcRequest : JsonRpcRequest<GetAccountRequest>
{
    public GetAccountJsonRpcRequest()
    {
        Method = JsonRpcMethod.GetAccount;
    }
}

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