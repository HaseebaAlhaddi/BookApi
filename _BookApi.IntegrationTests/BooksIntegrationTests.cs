using Microsoft.AspNetCore.Mvc.Testing;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using Xunit;

namespace BookApi.IntegrationTests;
public class BooksIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;
    public BooksIntegrationTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }
    [Fact]
public async Task GetBooks_WithoutToken_ReturnsUnauthorized()
{
    var response = await _client.GetAsync("/api/books");

    Assert.Equal(
        HttpStatusCode.Unauthorized,
        response.StatusCode);
}
}