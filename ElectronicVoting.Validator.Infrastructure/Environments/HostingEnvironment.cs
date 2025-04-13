using Microsoft.Extensions.Hosting;

namespace ElectronicVoting.Validator.Infrastructure.Environments;

public static class HostingEnvironment
{
    public static bool IsDevelopmentDockerTestOrDevelopment(this IHostEnvironment environment)
    {
        return environment.IsDevelopment() || environment.IsEnvironment("Development-Docker-Test");
    }
}