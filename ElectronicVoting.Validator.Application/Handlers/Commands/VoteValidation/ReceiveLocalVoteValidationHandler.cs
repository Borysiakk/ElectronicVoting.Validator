using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public record ReceiveLocalVoteValidationCommand :ISignedCommand, ITransaction
{
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
    public VoteValidationResultDto VoteValidationResult { get; set; }
}

[WolverineHandler]
public class ReceiveLocalVoteValidationHandler(
    IVoteValidationResultRepository voteValidationResultRepository,
    IVoteEncryptionRepository voteEncryptionRepository,
    IVoteValidationProcessRepository voteValidationProcessRepository,
    IElectionValidatorService electionValidatorService,
    ILocalVoteValidationProcessRepository localVoteValidationProcessRepository,
    IMessageBus bus)
{
    public async Task HandleAsync(ReceiveLocalVoteValidationCommand command, bool isValidSigned, CancellationToken cancellationToken)
    {
        if (!isValidSigned)
            return;
        
        var verifyValidationVote = await VerifyValidationVote(command, cancellationToken);
        if(verifyValidationVote is false)
            return;
        
        await SaveReceiveVoteValidationResult(command, cancellationToken);
        await CheckVoteThresholdReachedAsync(command, cancellationToken);;
    }

    private async Task<bool> VerifyValidationVote(ReceiveLocalVoteValidationCommand command, CancellationToken ct)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        
        var isVoteEncryptionExist = await voteEncryptionRepository.AnyExistsAsync(command.VoteValidationResult.VoteEncryptionId, ct);
        var validationProcessDocument = await voteValidationProcessRepository.GetByIdAsync(command.VoteValidationResult.VoteValidationProcessId, ct);
        var localValidationProcessDocument = await localVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(command.VoteValidationResult.VoteValidationProcessId, ct);
        var isExistVoteValidationResult = await voteValidationResultRepository.ExistsVoteValidationResultAsync(command.VoteValidationResult.VoteValidationProcessId, command.SignedByValidatorId, ct);

        if (validationProcessDocument is null)
            return false;
        
        if (localValidationProcessDocument is null)
            return false;
        
        if (validationProcessDocument.FinishedAt < DateTime.UtcNow)
            return false;   
        
        if (validationProcessDocument.Status is VoteValidationProcessStatus.Cancelled or VoteValidationProcessStatus.Completed or VoteValidationProcessStatus.Timeout)
            return false;
        
        if (localValidationProcessDocument.Status is LocalVoteValidationStatus.Cancelled or LocalVoteValidationStatus.Rejected or LocalVoteValidationStatus.Timeout)
            return false;
            
        if (isExistVoteValidationResult is true)
            return false;
        
        if (isVoteEncryptionExist is false)
            return false;
            
        return true;
    }
    
    private async Task SaveReceiveVoteValidationResult(ReceiveLocalVoteValidationCommand command, CancellationToken ct)
    {
        var localVoteValidationProcess = await localVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(command.VoteValidationResult.VoteValidationProcessId, ct);
        VoteValidationResultEntity validationResultEntity = new()
        {
            Id = Guid.NewGuid(),
            VoteEncryptionId = command.VoteValidationResult.VoteEncryptionId,
            VoteValidationProcessId = command.VoteValidationResult.VoteValidationProcessId,
            LocalVoteValidationProcessId = localVoteValidationProcess.Id,
            IsValid = command.VoteValidationResult.IsValid,
        };
        
        await voteValidationResultRepository.AddAsync(validationResultEntity, ct);
    }

    private async Task CheckVoteThresholdReachedAsync(ReceiveLocalVoteValidationCommand command, CancellationToken ct = default)
    {
        var localValidationProcessDocument = await localVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(command.VoteValidationResult.VoteValidationProcessId, ct);
        CheckLocalVoteThresholdReachedCommand checkVoteThresholdReachedCommand = new()
        {
            VoteEncryptionId = command.VoteValidationResult.VoteEncryptionId,
            LocalVoteValidationProcessId = localValidationProcessDocument.Id,
            VoteValidationProcessId = command.VoteValidationResult.VoteValidationProcessId,
        };
        
        await bus.SendAsync(checkVoteThresholdReachedCommand);
    }
}