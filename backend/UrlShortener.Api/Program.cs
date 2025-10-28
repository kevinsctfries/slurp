using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(opt =>
    opt.UseSqlite("Data Source=urls.db"));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
});

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.EnsureCreated();
}

app.UseCors("AllowFrontend");

var cleanupInterval = TimeSpan.FromHours(
    double.Parse(Environment.GetEnvironmentVariable("CLEANUP_INTERVAL_HOURS") ?? "1")
);
var cleanupThreshold = TimeSpan.FromHours(
    double.Parse(Environment.GetEnvironmentVariable("CLEANUP_THRESHOLD_HOURS") ?? "24")
);

var cancellationTokenSource = new CancellationTokenSource();
_ = Task.Run(async () => 
{
    while (!cancellationTokenSource.Token.IsCancellationRequested)
    {
        try
        {
            using var scope = app.Services.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var cutoff = DateTime.UtcNow - cleanupThreshold;
            var oldLinks = await db.Urls.Where(u => u.CreatedAt < cutoff).ToListAsync();
            if (oldLinks.Any())
            {
                db.Urls.RemoveRange(oldLinks);
                await db.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during cleanup: {ex.Message}");
        }

        await Task.Delay(cleanupInterval, cancellationTokenSource.Token);
    }
});

app.MapPost("/api/shorten", async (AppDbContext db, HttpRequest request) =>
{
    var body = await request.ReadFromJsonAsync<ShortenRequest>();
    if (body == null || string.IsNullOrWhiteSpace(body.Url))
        return Results.BadRequest("URL is required.");

    var shortCode = Guid.NewGuid().ToString().Substring(0, 6);
    var entry = new UrlEntry { ShortCode = shortCode, OriginalUrl = body.Url };
    db.Urls.Add(entry);
    await db.SaveChangesAsync();

    return Results.Ok(new { shortUrl = $"http://localhost:5124/s/{shortCode}" });
});

app.MapGet("/s/{code}", async (AppDbContext db, string code) =>
{
    var entry = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == code);
    if (entry == null) return Results.NotFound();
    return Results.Redirect(entry.OriginalUrl);
});

app.Lifetime.ApplicationStopping.Register(() => cancellationTokenSource.Cancel());

app.Run();

record ShortenRequest(string Url);

public partial class Program { }