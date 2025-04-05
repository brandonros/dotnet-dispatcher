using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Common.Attributes;
using Common.Model.JsonRpc;

namespace Common.Model.Requests;

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
