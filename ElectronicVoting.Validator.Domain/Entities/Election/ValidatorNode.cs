namespace ElectronicVoting.Validator.Domain.Entities.Election;

public class ValidatorNode : Entity<Guid>
{
    public string Name { get; set; }
    public string PublicKey { get; set; }
    public string ServerUrl { get; set; }
    public bool IsLeader { get; set; }
}