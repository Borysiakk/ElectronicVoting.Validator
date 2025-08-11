using Microsoft.Extensions.Configuration;
using Wolverine;
using Wolverine.RabbitMQ;
using Wolverine.RabbitMQ.Internal;

namespace ElectronicVoting.Validator.Infrastructure.RabbitMq;

public static class Extensions
{
    public static WolverineOptions UseRabbitMq(this WolverineOptions opts, IConfiguration configuration)
    {
        var rabbitMqSettings = new RabbitMqSettings();
        configuration.GetSection("RabbitMQ").Bind(rabbitMqSettings);
        
        opts.UseRabbitMq(rabbit =>
        {
            rabbit.Port = rabbitMqSettings.Port;
            rabbit.HostName = rabbitMqSettings.Host;
            rabbit.UserName = rabbitMqSettings.Username;
            rabbit.Password = rabbitMqSettings.Password;
        }).AutoProvision();
        
        //opts.PublishMessage<InitializeVoteValidationCommand>().ToRabbitQueue("election-vote-validation-queue");
        
        opts.ListenToRabbitQueue("election-vote-validation-queue", queue =>
        {
            queue.IsDurable = true;
            queue.QueueType = QueueType.classic;
        }).UseDurableInbox();
        return opts;
    }
}