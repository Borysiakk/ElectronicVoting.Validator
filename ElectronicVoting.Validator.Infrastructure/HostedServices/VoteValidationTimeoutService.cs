using ElectronicVoting.Validator.Infrastructure.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElectronicVoting.Validator.Infrastructure.HostedServices;

public class VoteValidationTimeoutService(IServiceScopeFactory scopeFactory) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            using var scope = scopeFactory.CreateScope();
            var  voteValidationTimeoutProcessor = scope.ServiceProvider.GetRequiredService<IVoteValidationTimeoutProcessor>();
            
            await voteValidationTimeoutProcessor.ProcessAsync(cancellationToken);
            await Task.Delay(TimeSpan.FromMinutes(1), cancellationToken);
        }
    }
}