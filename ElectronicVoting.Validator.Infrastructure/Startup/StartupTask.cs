using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElectronicVoting.Validator.Infrastructure.Startup;

public class StartupTask : IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    public StartupTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }


    public async Task StartAsync(CancellationToken cancellationToken)
    {

    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StartupTask: Kończenie działania.");
        return Task.CompletedTask;
    }
}