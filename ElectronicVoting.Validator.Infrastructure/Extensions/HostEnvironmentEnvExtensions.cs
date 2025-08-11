using Microsoft.Extensions.Hosting;

namespace ElectronicVoting.Validator.Infrastructure.Extensions;

public static class HostEnvironmentEnvExtensions
{
    private const string DevelopmentDockerName = "DevelopmentDocker";
    public static bool IsDevelopmentDocker(this IHostEnvironment env)
    {
        if (env is null) throw new ArgumentNullException(nameof(env));
        
        if (env.IsEnvironment(DevelopmentDockerName))
            return true;

        if (!env.IsDevelopment()) return false;
        
        var dotnetInContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER");
        if (!string.IsNullOrEmpty(dotnetInContainer) && dotnetInContainer != "0")
            return true;
            
        return File.Exists("/.dockerenv");
    }
}