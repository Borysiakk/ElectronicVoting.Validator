using ElectronicVoting.Validator.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ElectronicVoting.Validator.Infrastructure.HostedServices;

public class  PbftBlockCreatorService(ILogger<PbftBlockCreatorService> logger, IServiceScopeFactory scopeFactory)
    : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var  pbftBlockCreatorProcess = scope.ServiceProvider.GetRequiredService<IPbftBlockCreatorProcess>();
            await pbftBlockCreatorProcess.ProcessAsync(stoppingToken);
        
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}