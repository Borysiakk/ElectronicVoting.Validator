using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public static class Extensions
{
    public static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<INodeRepository, NodeRepository>();
        services.AddScoped<IBlockRepository, BlockRepository>();
        services.AddScoped<ITransactionRepository, TransactionRepository>();
        services.AddScoped<IPendingBlockRepository, PendingBlockRepository>();
        services.AddScoped<IPBFTSequenceRepository, PBFTSequenceRepository>();
        services.AddScoped<IBlockConfirmationRepository, BlockConfirmationRepository>();
        services.AddScoped<IApplicationSettingRepository, ApplicationSettingRepository>();
        services.AddScoped<IPendingTransactionRepository, PendingTransactionRepository>();
    }
}