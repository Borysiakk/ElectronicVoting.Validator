using ElectronicVoting.Validator.Domain.Interface;

namespace ElectronicVoting.Validator.Domain.Models.Election;

public class VoteEncryption : ISignable
{
    public VoteEncryptionDetails VoteEncryptionDetails { get; set; }
    public VoteProofOfKnowledgeBase VoteProofOfKnowledgeBase { get; set; }
}