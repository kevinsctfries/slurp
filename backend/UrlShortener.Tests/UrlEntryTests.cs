using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Api.Data;
using UrlShortener.Api.Models;
using Xunit;

namespace UrlShortener.Tests
{
    public class UrlEntryTests
    {
        private AppDbContext CreateInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new AppDbContext(options);
        }

        [Fact]
        public void UrlEntry_DefaultValues_AreInitialized()
        {
            var entry = new UrlEntry();

            Assert.Equal(string.Empty, entry.ShortCode);
            Assert.Equal(string.Empty, entry.OriginalUrl);
            Assert.True((DateTime.UtcNow - entry.CreatedAt).TotalSeconds < 1, "CreatedAt should default to now");
        }

        [Fact]
        public async Task UrlEntry_CanBeSavedAndRetrieved_FromDatabase()
        {
            using var db = CreateInMemoryDb();

            var entry = new UrlEntry
            {
                ShortCode = "abc123",
                OriginalUrl = "https://example.com"
            };

            db.Urls.Add(entry);
            await db.SaveChangesAsync();

            var saved = await db.Urls.FirstOrDefaultAsync(u => u.ShortCode == "abc123");

            Assert.NotNull(saved);
            Assert.Equal("https://example.com", saved!.OriginalUrl);
            Assert.Equal("abc123", saved.ShortCode);
        }

        [Fact]
        public async Task UrlEntry_CreatedAt_RemainsConsistent_WhenSaved()
        {
            using var db = CreateInMemoryDb();
            var createdAt = DateTime.UtcNow;

            var entry = new UrlEntry
            {
                ShortCode = "xyz789",
                OriginalUrl = "https://persisted.com",
                CreatedAt = createdAt
            };

            db.Urls.Add(entry);
            await db.SaveChangesAsync();

            var saved = await db.Urls.FirstAsync(u => u.ShortCode == "xyz789");

            Assert.Equal(createdAt, saved.CreatedAt);
        }
    }
}
