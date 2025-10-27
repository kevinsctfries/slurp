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

app.MapPost("/api/shorten", async (AppDbContext db, string url) =>
{
    var shortCode = Guid.NewGuid().ToString().Substring(0, 6);
    var entry = new UrlEntry { ShortCode = shortCode, OriginalUrl = url };
    db.Urls.Add(entry);
    await db.SaveChangesAsync();
    return Results.Ok(new { shortUrl = $"http://localhost:5124/s/{shortCode}"});
});

app.MapGet("/s/{code}", async (AppDbContext db, string code) =>
{
    var entry = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == code);
    if (entry == null) return Results.NotFound();
    return Results.Redirect(entry.OriginalUrl);
});

app.Run();