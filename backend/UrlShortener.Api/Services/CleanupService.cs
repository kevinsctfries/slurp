using Microsoft.Extensions.Hosting;
using UrlShortener.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace UrlShortener.Api.Services
{
    public class CleanupService : BackgroundService
    {
        private readonly IServiceProvider _services;
        private readonly TimeSpan _interval = TimeSpan.FromHours(1);

        public CleanupService(IServiceProvider services)
        {
            _services = services;
        }
        
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _services.CreateScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                
                var cutoff = DateTime.UtcNow.AddHours(-24);
                var oldLinks = await context.Urls
                    .Where(u => u.CreatedAt < cutoff)
                    .ToListAsync(stoppingToken);
                
                if (oldLinks.Any())
                {
                    context.Urls.RemoveRange(oldLinks);
                    await context.SaveChangesAsync(stoppingToken);
                }

                await Task.Delay(_interval, stoppingToken);
            }
        }
    }
}