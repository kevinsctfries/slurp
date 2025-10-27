using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Models;

namespace UrlShortener.Api.Data;

public class AppDbContext : DbContext
{
    public DbSet<UrlEntry> Urls => Set<UrlEntry>();

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }
}