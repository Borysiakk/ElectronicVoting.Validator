using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

public static class Extensions
{
    public static void AddRepositoriesForValidatorLedger(this IServiceCollection services)
    {
        services.AddScoped<ILocalVoteValidationProcessRepository, LocalVoteValidationProcessRepository>();
        services.AddScoped<IVoteConsensusConfirmationRepository, VoteConsensusConfirmationRepository>();
        services.AddScoped<IVoteEncryptionRepository, VoteEncryptionRepository>();
        services.AddScoped<IVoteValidationProcessRepository, VoteValidationProcessRepository>();
        services.AddScoped<IVoteValidationResultRepository, VoteValidationResultRepository>();
        services.AddScoped<IPendingBlockRepository, PendingBlockRepository>();
        services.AddScoped<IPendingTransactionRepository, PendingTransactionRepository>();
        services.AddScoped<IBlockRepository, BlockRepository>();
        services.AddScoped<IPbftSequenceRepository, PbftSequenceRepository>();
    }
}