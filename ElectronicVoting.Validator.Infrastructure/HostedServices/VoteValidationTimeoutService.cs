using ElectronicVoting.Validator.Domain.Interface.Processes;
using ElectronicVoting.Validator.Infrastructure.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

namespace ElectronicVoting.Validator.Infrastructure.HostedServices;

public class VoteValidationTimeoutService(IServiceScopeFactory scopeFactory, IOptions<BackgroundProcessOptions> backgroundProcessOptions ) : BackgroundService
{
    private readonly BackgroundProcessOptions _backgroundProcessOptions = backgroundProcessOptions.Value;
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var  voteValidationTimeoutProcessor = scope.ServiceProvider.GetRequiredService<IVoteValidationTimeoutProcessor>();
            
            await voteValidationTimeoutProcessor.ProcessAsync(cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(_backgroundProcessOptions.VoteValidationTimeoutIntervalMinutes), cancellationToken);
        }
    }
}