using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public record FinalizeLocalVoteValidationCommand :ISignedCommand, ITransaction
{
    public Guid VoteValidationProcessId { get; set; }
    public Guid SignedByValidatorId { get; set; }
    public string Signature { get; set; }
}

public class FinalizeLocalVoteValidationHandler(
    IVoteValidationProcessRepository voteValidationProcessRepository,
    ILocalVoteValidationProcessRepository localVoteValidationProcessRepository)
{
    public async Task HandleAsync(FinalizeLocalVoteValidationCommand command, bool isValidSigned, CancellationToken ct)
    {
        if(isValidSigned)
            return;
        
        await ChangeStatusVoteValidationProcessAsync(command, ct);
        await ChangeStatusLocalVoteValidationProcessAsync(command, ct);
    }

    private async Task ChangeStatusVoteValidationProcessAsync(FinalizeLocalVoteValidationCommand command, CancellationToken ct)
    {
        var validationProcessDocument = await voteValidationProcessRepository.GetByIdAsync(command.VoteValidationProcessId, ct);
        validationProcessDocument.Status = VoteValidationProcessStatus.ReadyToCommit;
        
        await voteValidationProcessRepository.UpdateAsync(validationProcessDocument, ct);
    }
    
    private async Task ChangeStatusLocalVoteValidationProcessAsync(FinalizeLocalVoteValidationCommand command, CancellationToken ct)
    {
        var localVoteValidationProcess = await localVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(command.VoteValidationProcessId, ct);
        localVoteValidationProcess.Status = LocalVoteValidationStatus.Completed;
        
        await localVoteValidationProcessRepository.UpdateAsync(localVoteValidationProcess, ct);
    }
}