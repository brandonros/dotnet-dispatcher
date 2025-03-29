using System.Text.Json.Serialization;

namespace Dispatcher.Model.Requests;

/// <summary>
/// Parameters for the ping method
/// </summary>
public class PingRequest
{
    /// <summary>
    /// Optional message to echo back
    /// </summary>
    /// <example>Hello, World!</example>
    [JsonPropertyName("message")]
    public string Message { get; set; }
}
