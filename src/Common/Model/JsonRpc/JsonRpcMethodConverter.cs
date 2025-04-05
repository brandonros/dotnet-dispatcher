using System.Text.Json;
using System.Text.Json.Serialization;

namespace Common.Model.JsonRpc;

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
