using Hangfire;
using Hangfire.Redis.StackExchange;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Hangfire;

public static class Extensions
{
    public static void AddHangfire(this IServiceCollection service)
    {
        var redisConnectionStrings = Environment.GetEnvironmentVariable("REDIS_URL");
        service.AddHangfire(config => config.UseRedisStorage(redisConnectionStrings, new RedisStorageOptions { }));
        service.AddHangfireServer(option =>
        {
            option.WorkerCount = 1;
            option.Queues = ["Queue.Block"];
        });
        
        service.AddHangfireServer(option =>
        {
            option.WorkerCount = 1;
            option.Queues = ["Queue.PendingTransaction"];
        });
        
        service.AddHangfireServer(option =>
        {
            option.WorkerCount = 10;
            option.Queues = ["default"];
        });
        
    }
}