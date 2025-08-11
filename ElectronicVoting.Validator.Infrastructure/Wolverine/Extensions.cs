using System.Reflection;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.RabbitMq;
using ElectronicVoting.Validator.Infrastructure.Wolverine.Middleware;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wolverine;
using Wolverine.EntityFrameworkCore;
using Wolverine.Http;
using Wolverine.Postgresql;

namespace ElectronicVoting.Validator.Infrastructure.Wolverine;

public static class Extensions
{
    public static IServiceCollection AddWolverine(this IServiceCollection service, IConfiguration configuration, params Assembly[] applicationAssembly)
    {
        service.AddWolverineHttp();
        service.AddWolverine(opts =>
        {
            foreach (var assembly in applicationAssembly)
                opts.Discovery.IncludeAssembly(assembly);
            
            opts.UseRabbitMq(configuration);
            
            opts.Policies
                .ForMessagesOfType<ISignedCommand>()
                .AddMiddleware(typeof(SignatureValidationMiddleware));
            
            opts.Policies
                .ForMessagesOfType<ITransaction>()
                .AddMiddleware(typeof(UnitOfWorkMiddleware));
        });
        return service;
    }
}