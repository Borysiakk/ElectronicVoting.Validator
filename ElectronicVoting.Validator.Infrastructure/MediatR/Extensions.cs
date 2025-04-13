using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.MediatR;

public static class Extensions
{
    public static void AddMediatR(this IServiceCollection services, params Assembly[] applicationAssembly)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(applicationAssembly));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));
    }
}