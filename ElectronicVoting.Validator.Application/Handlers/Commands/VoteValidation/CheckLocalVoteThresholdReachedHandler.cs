
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public record CheckLocalVoteThresholdReachedCommand: ITransaction
{
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
    public Guid LocalVoteValidationProcessId { get; set; }
}

public class CheckLocalVoteThresholdReachedHandler(
    IValidatorNodeRepository validatorNodeRepository,
    IVoteValidationResultRepository voteValidationResultRepository,
    ILocalVoteValidationProcessRepository localVoteValidationProcessRepository,
    IValidationVoteApiService validationVoteApiService,
    IElectionValidatorService electionValidatorService,
    ISignatureService signatureService)
{
    public async Task HandleAsync(CheckLocalVoteThresholdReachedCommand command, CancellationToken ct)
    {
        var countValidator = await validatorNodeRepository.CountAsync(ct);
        var countVoteValidationResult = await voteValidationResultRepository.CountResultsForLocalValidationProcessAsync(command.LocalVoteValidationProcessId, ct);
        if (countVoteValidationResult >= countValidator / 2)
        {
            await ChangeStatusLocalVoteValidationProcess(command.LocalVoteValidationProcessId, ct);
            await NotifyLeaderOfVoteConsensusAsync(command, ct);
        }
    }

    private async Task ChangeStatusLocalVoteValidationProcess(Guid localVoteValidationProcessId, CancellationToken cancellationToken = default)
    {
        var localVoteValidationProcess = await localVoteValidationProcessRepository.GetByIdAsync(localVoteValidationProcessId, cancellationToken);
        localVoteValidationProcess.Status = LocalVoteValidationStatus.Completed;
        
        await localVoteValidationProcessRepository.UpdateAsync(localVoteValidationProcess, cancellationToken);
    }

    private async Task NotifyLeaderOfVoteConsensusAsync(CheckLocalVoteThresholdReachedCommand command, CancellationToken cancellationToken = default)
    {
        var currentValidatorId= await electionValidatorService.GetCurrentValidatorIdAsync(cancellationToken);
        
        LeaderReceiveVoteConsensusReportCommand consensusReportCommand = new()
        {
            SignedByValidatorId = currentValidatorId,
            VoteEncryptionId = command.VoteEncryptionId,
            VoteValidationProcessId = command.VoteValidationProcessId,
        };
        
        consensusReportCommand.Signature = signatureService.Sign(consensusReportCommand);
        await validationVoteApiService.BroadcastLeaderReceiveVoteConsensusReportAsync(consensusReportCommand, cancellationToken);
    }

}