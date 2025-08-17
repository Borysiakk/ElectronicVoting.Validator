using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Models.Election;

namespace ElectronicVoting.Validator.Application.DTO.BlockValidation;

public class PendingBlockDetailsDto :IHashable
{
    public Guid Id { get; set; }
    public string Hash { get; set; }
    public string PreviousHash { get; set; }
    
    public Guid ValidatorNodeId { get; set; } 
    public PbftSequenceDto PbftSequence { get; set; }
    public List<PendingTransactionDetailsDto> PendingTransactions { get; set; }
    
    public object GetHashSource()
    {
        return new
        {
            Id,
            Hash,
            PreviousHash,
            PbftSequence,
            PendingTransactions,
        };
    }
}
