using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

namespace ElectronicVoting.Validator.Infrastructure.Services;

public interface IVoteValidationTimeoutProcessor
{
    Task ProcessAsync(CancellationToken ct);
}

public class VoteValidationTimeoutProcessor(
    IUnitOfWork unitOfWork,
    IVoteValidationProcessRepository voteValidationProcessRepository,
    ILocalVoteValidationProcessRepository localVoteValidationProcessRepository)
    : IVoteValidationTimeoutProcessor
{
    public async Task ProcessAsync(CancellationToken ct)
    {
        await unitOfWork.BeginTransaction(ct);
        
        var votesValidationProcess =
            await voteValidationProcessRepository.GetReadyForTimeoutAsync(DateTime.UtcNow, ct);
        foreach (var voteValidationProcess in votesValidationProcess)
            voteValidationProcess.Status = VoteValidationProcessStatus.Timeout;

        var localVotesValidationProcess = 
            await localVoteValidationProcessRepository.GetByVoteValidationProcessIdsAsync(votesValidationProcess.Select(a => a.Id).ToArray(), ct);
        
        foreach (var localVoteValidationProcess in localVotesValidationProcess)
            localVoteValidationProcess.Status = LocalVoteValidationStatus.Timeout;

        if (votesValidationProcess.Any())
            await voteValidationProcessRepository.UpdateAsync(votesValidationProcess, ct);

        if (localVotesValidationProcess.Any())
            await localVoteValidationProcessRepository.UpdateAsync(localVotesValidationProcess.ToArray(), ct);

        await unitOfWork.Commit(ct);
    }
}