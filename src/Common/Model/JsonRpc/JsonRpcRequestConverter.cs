using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Model.Requests;

namespace Common.Model.JsonRpc;

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
