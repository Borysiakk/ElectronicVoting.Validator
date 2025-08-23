namespace ElectronicVoting.Validator.Infrastructure.Configuration;

public class ConsensusOptions
{
    public const string SectionName = "ConsensusProcess";
    public int MinimumValidValidations { get; set; } = 2;
}