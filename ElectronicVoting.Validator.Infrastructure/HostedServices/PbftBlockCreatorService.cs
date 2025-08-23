using ElectronicVoting.Validator.Domain.Interface.Processes;
using ElectronicVoting.Validator.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ElectronicVoting.Validator.Infrastructure.HostedServices;

public class  PbftBlockCreatorService(ILogger<PbftBlockCreatorService> logger, IServiceScopeFactory scopeFactory, IOptions<BackgroundProcessOptions> backgroundProcessOptions )
    : BackgroundService
{
    private readonly BackgroundProcessOptions _backgroundProcessOptions = backgroundProcessOptions.Value;
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromSeconds(_backgroundProcessOptions.BlockValidationTimeoutIntervalMinutes), stoppingToken);
            
            using var scope = scopeFactory.CreateScope();
            var  pbftBlockCreatorProcess = scope.ServiceProvider.GetRequiredService<IPbftBlockCreatorProcess>();
            await pbftBlockCreatorProcess.ProcessAsync(stoppingToken);
            
        }
    }
}