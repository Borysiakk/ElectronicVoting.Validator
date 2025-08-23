namespace ElectronicVoting.Validator.Infrastructure.Configuration;

public class BackgroundProcessOptions
{
    public const string SectionName = "BackgroundProcess";
    public int VoteValidationTimeoutIntervalMinutes { get; set; } = 5;
    public int BlockValidationTimeoutIntervalMinutes { get; set; } = 1;
}