namespace ElectronicVoting.Validator.Infrastructure.Configuration;


public class BlockValidationOptions
{
    public const string SectionName = "BlockValidation";
    
    public int BlockValidationExpiryMinutes { get; set; } = 15;
}
