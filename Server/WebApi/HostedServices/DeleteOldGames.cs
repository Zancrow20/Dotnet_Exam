using DataAccess;
using Microsoft.EntityFrameworkCore;

namespace WebApi.HostedServices;

public class DeleteOldGames : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly PeriodicTimer _timer = new(TimeSpan.FromMinutes(30));
    
    public DeleteOldGames(IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        
        while (await _timer.WaitForNextTickAsync(stoppingToken)
               && !stoppingToken.IsCancellationRequested)
        {
            var count = await dbContext.Games
                .Where(g => g.Date <= DateTime.Now - TimeSpan.FromHours(24) &&
                            g.PlayerOne == null &&
                            g.PlayerTwo == null)
                .ExecuteDeleteAsync(cancellationToken: stoppingToken);
        }
    }
}