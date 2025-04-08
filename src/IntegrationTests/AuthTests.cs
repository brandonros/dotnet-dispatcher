using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace IntegrationTests;

public class AuthTests
{
    private readonly HttpClient _httpClient;
    private readonly ITestOutputHelper _output;

    public AuthTests(ITestOutputHelper output)
    {
        _httpClient = new HttpClient();
        _output = output;
    }

    [Fact]
    public async Task Auth_GetToken_ShouldReturnToken()
    {
        // Arrange
        var request = new
        {
            username = "test",
            password = "test"
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("http://auth_proxy:5002/auth/token", request);
        var responseString = await response.Content.ReadAsStringAsync();
        _output.WriteLine($"Response: {responseString}"); // Log the actual response

        var content = await response.Content.ReadFromJsonAsync<JsonElement>();

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        // Log the response structure
        _output.WriteLine($"Response properties: {string.Join(", ", content.EnumerateObject().Select(p => p.Name))}");

        // Assert token exists and is not empty
        Assert.True(content.TryGetProperty("token", out var tokenElement), "Response should contain a token property");
        var token = tokenElement.GetString();
        Assert.NotNull(token);
        Assert.NotEmpty(token);
        
        // Verify token is in JWT format (three parts separated by dots)
        var tokenParts = token.Split('.');
        Assert.Equal(3, tokenParts.Length);
    }

    [Fact]
    public async Task Auth_GetToken_WithInvalidCredentials_ShouldReturnUnauthorized()
    {
        // Arrange
        var request = new
        {
            username = "wrong",
            password = "wrong"
        };

        // Act
        var response = await _httpClient.PostAsJsonAsync("http://auth_proxy:5002/auth/token", request);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }
}