using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace IntegrationTests;

public class SwaggerEndpointTests
{
    private readonly HttpClient _httpClient;

    public SwaggerEndpointTests()
    {
        _httpClient = new HttpClient();
    }

    [Fact]
    public async Task Dispatcher_SwaggerEndpoint_ShouldBeAccessible()
    {
        // Act
        var response = await _httpClient.GetAsync("http://dispatcher:5000/swagger/index.html");
        var jsonResponse = await _httpClient.GetAsync("http://dispatcher:5000/swagger/v1/swagger.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, jsonResponse.StatusCode);
    }

    [Fact]
    public async Task Consumer_SwaggerEndpoint_ShouldBeAccessible()
    {
        // Act
        var response = await _httpClient.GetAsync("http://consumer:5001/swagger/index.html");
        var jsonResponse = await _httpClient.GetAsync("http://consumer:5001/swagger/v1/swagger.json");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, jsonResponse.StatusCode);
    }
}