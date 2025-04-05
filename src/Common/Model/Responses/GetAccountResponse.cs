using System.Text.Json.Serialization;

namespace Common.Model.Responses;


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
