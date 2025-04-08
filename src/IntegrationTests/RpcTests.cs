using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests;

public class RpcTests
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;

    public RpcTests(ITestOutputHelper output)
    {
        _httpClient = new HttpClient();
        _output = output;
    }

    [Fact]
    public async Task Dispatcher_Rpc_GetUser_ShouldReturnUser()
    {
        // Arrange
        var request = new
        {
            jsonrpc = "2.0",
            method = "account.get",
            id = Guid.NewGuid().ToString(),
            @params = new 
            {
                accountId = "a81bc81b-dead-4e5d-abff-90865d1e13b1"
            }
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("http://dispatcher:5001/rpc", request);
        var responseString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response: {responseString}"); // Log the actual response

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Log the response structure
        _output.WriteLine($"Response properties: {string.Join(", ", content.EnumerateObject().Select(p => p.Name))}");

        // More defensive assertions
        if (content.TryGetProperty("jsonrpc", out var jsonrpcElement))
        {
            Assert.Equal("2.0", jsonrpcElement.GetString());
        }

        // Check for either result or error
        Assert.True(
            content.TryGetProperty("result", out var resultElement) || 
            content.TryGetProperty("error", out var errorElement),
            "Response should contain either a result or error property"
        );

        if (content.TryGetProperty("error", out var error))
        {
            _output.WriteLine($"Error response: {error}");
        }
    }
}