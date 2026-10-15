using System.Net.Http.Json;
using Bookstore.Api.Contracts;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.MongoDb;

namespace Bookstore.Api.IntegrationTests;

public class BookstoreHealthTests : IAsyncLifetime
{
    private readonly MongoDbContainer _mongoDbContainer = new MongoDbBuilder()
        .WithImage("mongo:7.0")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;

    public async Task InitializeAsync()
    {
        await _mongoDbContainer.StartAsync();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            builder.UseSetting("MONGODB_URI", _mongoDbContainer.GetConnectionString()));
        _client = _factory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        _client.Dispose();
        _factory.Dispose();
        await _mongoDbContainer.DisposeAsync();
    }

    [Fact]
    public async Task GetBooksHealth_ReturnsHealthyStatus()
    {
        var response = await _client.GetAsync("/api/v1/books/health");

        response.EnsureSuccessStatusCode();

        var body = await response.Content.ReadFromJsonAsync<HealthResponse>();

        Assert.NotNull(body);
        Assert.Equal("healthy", body!.Status);
    }
}
