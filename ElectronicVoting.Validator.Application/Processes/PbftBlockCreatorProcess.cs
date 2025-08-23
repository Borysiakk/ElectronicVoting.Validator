using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface.Processes;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine;

namespace ElectronicVoting.Validator.Application.Processes;

public class PbftBlockCreatorProcess(
    IMessageBus messageBus,
    IPendingBlockFactory pendingBlockFactory,
    IVoteValidationProcessRepository voteValidationProcessRepository)
    : IPbftBlockCreatorProcess
{
    public async Task ProcessAsync(CancellationToken ct)
    {
        var validationProcessWhenReadyToCommit = await GetVoteValidationProcessWhenReadyToCommitAsync(ct);
        if (!validationProcessWhenReadyToCommit.Any())
            return;
        
        var pendingBlockDetailsResult = await pendingBlockFactory.TryCreatePendingBlockAsync(validationProcessWhenReadyToCommit, ct);
        if (pendingBlockDetailsResult.IsFailed)
            return;
        
        var command = CreateBlockValidationCommand(pendingBlockDetailsResult.Value);
        await messageBus.SendAsync(command);
    }
    
    private LeaderInitiateBlockValidationCommand CreateBlockValidationCommand(PendingBlockDetailsDto pendingBlockDetails)
    {
        return new LeaderInitiateBlockValidationCommand
        {
            PendingBlockDetails = pendingBlockDetails
        };
    }
    
    private async Task<IReadOnlyList<VoteValidationProcessEntity>> GetVoteValidationProcessWhenReadyToCommitAsync(CancellationToken ct)
    {
        var votesValidationProcess = await voteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, ct);

        foreach (var voteValidationProcess in votesValidationProcess)
            voteValidationProcess.Status = VoteValidationProcessStatus.ProcessedToCommit;
         
        await voteValidationProcessRepository.UpdateAsync(votesValidationProcess.ToArray(), ct);
        
        return votesValidationProcess;
    }
}

