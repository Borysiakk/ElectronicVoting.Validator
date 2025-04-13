namespace ElectronicVoting.Validator.Domain.Models;

public record VoteProofOfKnowledgeBase
{
    public string[] E { get; set; }
    public string[] U { get; set; }
    public string[] V { get; set; }
}