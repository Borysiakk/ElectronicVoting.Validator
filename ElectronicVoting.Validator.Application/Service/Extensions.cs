using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Application.Service;

public static  class Extensions
{
    public static void AddServices(this IServiceCollection services)
    {
        services.AddScoped<IVoteValidationService, VoteValidationService>();
        services.AddScoped<IElectionConsensusService, ElectionConsensusService>();
    }
}