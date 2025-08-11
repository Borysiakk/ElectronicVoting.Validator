namespace ElectronicVoting.Validator.Infrastructure.RabbitMq;

public record RabbitMqSettings
{
    public int Port { get; set; }
    public string Host { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
}
