using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;
using UrlShortener.Api.Services;
using Xunit;

namespace UrlShortener.Tests
{
    public class CleanupServiceTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        private IServiceProvider CreateServiceProvider(AppDbContext dbContext)
        {
            var services = new ServiceCollection();
            services.AddSingleton(dbContext);
            services.AddScoped<AppDbContext>(_ => dbContext);
            return services.BuildServiceProvider();
        }

        [Fact]
        public async Task CleanupService_Removes_Old_Urls()
        {
            // Arrange
            var db = CreateInMemoryDb();

            db.Urls.AddRange(
                new UrlEntry { OriginalUrl = "https://old.com", ShortCode = "old123", CreatedAt = DateTime.UtcNow.AddHours(-25) },
                new UrlEntry { OriginalUrl = "https://new.com", ShortCode = "new123", CreatedAt = DateTime.UtcNow }
            );
            await db.SaveChangesAsync();

            var provider = CreateServiceProvider(db);
            var cleanupService = new CleanupService(provider);

            using var scope = provider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            var cutoff = DateTime.UtcNow.AddHours(-24);
            var oldLinks = await context.Urls
                .Where(u => u.CreatedAt < cutoff)
                .ToListAsync();

            context.Urls.RemoveRange(oldLinks);
            await context.SaveChangesAsync();

            var urls = await db.Urls.ToListAsync();
            Assert.Single(urls);
            Assert.Equal("https://new.com", urls[0].OriginalUrl);
        }
    }
}
