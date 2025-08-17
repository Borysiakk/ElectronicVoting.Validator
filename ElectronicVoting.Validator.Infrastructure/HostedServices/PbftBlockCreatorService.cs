using ElectronicVoting.Validator.Domain.Interface.Processes;
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
        
            await Task.Delay(TimeSpan.FromSeconds(15), stoppingToken);
        }
    }
}