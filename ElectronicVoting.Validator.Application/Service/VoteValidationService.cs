using ElectronicVoting.Validator.Domain.Models;

namespace ElectronicVoting.Validator.Application.Service;


public interface IVoteValidationService
{
    Task<bool> ValidateVoteAsync(VoteEncryption voteEncryption) ;
}

public class VoteValidationService: IVoteValidationService
{
    public Task<bool> ValidateVoteAsync(VoteEncryption voteEncryption)
    {
        Task.Delay(4000).Wait();
        return Task.FromResult(true);
    }
}