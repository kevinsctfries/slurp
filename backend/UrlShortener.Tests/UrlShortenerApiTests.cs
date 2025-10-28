using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;
using Xunit;

namespace UrlShortener.Api.Tests;

public class UrlShortenerApiTests : IClassFixture<WebApplicationFactory<Program>>, IDisposable
{
    private readonly WebApplicationFactory<Program> _factory;
    private readonly HttpClient _client;
    private readonly SqliteConnection _connection;

    public UrlShortenerApiTests()
    {
        _connection = new SqliteConnection("DataSource=:memory:");
        _connection.Open();

        _factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType == typeof(DbContextOptions<AppDbContext>));
                    if (descriptor != null) services.Remove(descriptor);

                    services.AddDbContext<AppDbContext>(options =>
                        options.UseSqlite(_connection));

                    using var scope = services.BuildServiceProvider().CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                    db.Database.EnsureCreated();
                });
            });

        _client = _factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task Post_Shorten_ValidUrl_ReturnsShortUrl()
    {
        var originalUrl = "https://example.com/very/long/url";

        var response = await _client.PostAsJsonAsync("/api/shorten", new { url = originalUrl });
        var json = await response.Content.ReadFromJsonAsync<JsonElement>();

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.True(json.TryGetProperty("shortUrl", out var shortUrlProp));
        var shortUrl = shortUrlProp.GetString()!;
        Assert.StartsWith("http://localhost:5124/s/", shortUrl); // Match port!
        Assert.Equal(6, shortUrl.Split('/').Last().Length);
    }

    [Fact]
    public async Task Post_Shorten_MultipleUrls_GeneratesUniqueShortCodes()
    {
        var urls = new[] { "https://a.com", "https://b.com", "https://c.com" };
        var codes = new HashSet<string>();

        foreach (var url in urls)
        {
            var resp = await _client.PostAsJsonAsync("/api/shorten", new { url });
            var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
            var code = json.GetProperty("shortUrl").GetString()!.Split('/').Last();
            codes.Add(code);
        }

        Assert.Equal(urls.Length, codes.Count);
    }

    [Fact]
    public async Task Get_Redirect_ExistingShortCode_RedirectsToOriginalUrl()
    {
        var originalUrl = "https://redirect-test.com";

        var shortenResp = await _client.PostAsJsonAsync("/api/shorten", new { url = originalUrl });
        var json = await shortenResp.Content.ReadFromJsonAsync<JsonElement>();
        var shortCode = json.GetProperty("shortUrl").GetString()!.Split('/').Last();

        var redirectResp = await _client.GetAsync($"/s/{shortCode}");

        Assert.Equal(HttpStatusCode.Redirect, redirectResp.StatusCode);
        Assert.Equal(originalUrl, redirectResp.Headers.Location?.OriginalString);
    }

    [Fact]
    public async Task Get_Redirect_NonExistentShortCode_ReturnsNotFound()
    {
        var resp = await _client.GetAsync("/s/nonexistent");
        Assert.Equal(HttpStatusCode.NotFound, resp.StatusCode);
    }

    [Fact]
    public async Task Post_Shorten_EmptyUrl_ReturnsBadRequest()
    {
        var resp = await _client.PostAsJsonAsync("/api/shorten", new { url = "" });
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Post_Shorten_MissingUrl_ReturnsBadRequest()
    {
        var jsonContent = JsonContent.Create(new { });
        var resp = await _client.PostAsync("/api/shorten", jsonContent);
        Assert.Equal(HttpStatusCode.BadRequest, resp.StatusCode);
    }

    [Fact]
    public async Task Database_Persists_UrlEntry_WithCorrectCreatedAt()
    {
        var originalUrl = "https://persistence-test.com";
        var before = DateTime.UtcNow.AddSeconds(-1);

        var resp = await _client.PostAsJsonAsync("/api/shorten", new { url = originalUrl });
        var json = await resp.Content.ReadFromJsonAsync<JsonElement>();
        var shortCode = json.GetProperty("shortUrl").GetString()!.Split('/').Last();

        using var scope = _factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var entry = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == shortCode);

        var after = DateTime.UtcNow.AddSeconds(1);

        Assert.NotNull(entry);
        Assert.Equal(originalUrl, entry.OriginalUrl);
        Assert.True(entry.CreatedAt >= before && entry.CreatedAt <= after);
    }

    public void Dispose()
    {
        _client.Dispose();
        _factory.Dispose();
        _connection.Close();
        _connection.Dispose();
    }
}