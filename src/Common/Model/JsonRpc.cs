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