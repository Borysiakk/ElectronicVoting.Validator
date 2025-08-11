using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public record LeaderReceiveVoteConsensusReportCommand: ISignedCommand, ITransaction
{
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
    
    public Guid ValidatorId { get; set; }
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
}

[WolverineHandler]
public class LeaderReceiveVoteConsensusReportHandler(
    IVoteConsensusConfirmationRepository voteConsensusConfirmationRepository,
    IValidatorNodeRepository validatorNodeRepository,
    IVoteValidationProcessRepository voteValidationProcessRepository,
    IValidationVoteApiService validationVoteApiService,
    IElectionValidatorService electionValidatorService,
    ILocalVoteValidationProcessRepository localVoteValidationProcessRepository,
    ISignatureService signatureService)
{
    [WolverineHandler]
    public async Task HandleAsync(LeaderReceiveVoteConsensusReportCommand command, bool isValidSigned, CancellationToken ct)
    {
        if (!isValidSigned)
            return;
        
        await CreateAndSaveVoteConsensusReport(command, ct);
        if (await CheckVoteConsensusThresholdAsync(command.VoteValidationProcessId, ct))
        {
            await ChangeStatusVoteValidationProcessAsync(command.VoteValidationProcessId, ct);
            await ChangeStatusLocalVoteValidationProcessAsync(command.VoteValidationProcessId, ct);
            await BroadcastFinalizeVoteValidationAsync(command.VoteValidationProcessId, ct);
        }
    }

    private async Task BroadcastFinalizeVoteValidationAsync(Guid voteValidationProcessId, CancellationToken ct = default)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        
        FinalizeLocalVoteValidationCommand finalizeLocalVoteValidationCommand = new()
        {
            SignedByValidatorId = currentValidatorId,
            VoteValidationProcessId = voteValidationProcessId,
        };
        var signature = signatureService.Sign(finalizeLocalVoteValidationCommand);
        finalizeLocalVoteValidationCommand.Signature = signature;
        
        await validationVoteApiService.BroadcastFinalizeLocalVoteValidationAsync(finalizeLocalVoteValidationCommand, ct);
    }
    
    private async Task CreateAndSaveVoteConsensusReport(LeaderReceiveVoteConsensusReportCommand command, CancellationToken ct = default)
    {
        VoteConsensusConfirmationEntity voteConsensusConfirmationEntity = new()
        {
            Id = Guid.NewGuid(),
            ValidatorId = command.ValidatorId,
            VoteEncryptionId = command.VoteEncryptionId,
            VoteValidationProcessId = command.VoteValidationProcessId,
        };
        await voteConsensusConfirmationRepository.AddAsync(voteConsensusConfirmationEntity, ct);
    }

    private async Task<bool> CheckVoteConsensusThresholdAsync(Guid voteValidationProcessId, CancellationToken ct = default)
    {
        var countValidator = await validatorNodeRepository.CountAsync(ct);
        var countConsensusConfirmation = await voteConsensusConfirmationRepository.CountByVoteValidationProcessId(voteValidationProcessId, ct);

        return countConsensusConfirmation >= countValidator / 2;
    }

    private async Task ChangeStatusVoteValidationProcessAsync(Guid voteValidationProcessId, CancellationToken ct)
    {
       var validationProcess = await voteValidationProcessRepository.GetByIdAsync(voteValidationProcessId, ct);
       validationProcess.Status = VoteValidationProcessStatus.ReadyToCommit;
       
       await voteValidationProcessRepository.UpdateAsync(validationProcess, ct);
    }
    
    private async Task ChangeStatusLocalVoteValidationProcessAsync(Guid voteValidationProcessId, CancellationToken ct)
    {
        var localVoteValidationProcess = await localVoteValidationProcessRepository.GetByVoteValidationProcessIdAsync(voteValidationProcessId, ct);
        if(localVoteValidationProcess is null)
            return;
        
        localVoteValidationProcess.Status = LocalVoteValidationStatus.Completed;
        await localVoteValidationProcessRepository.UpdateAsync(localVoteValidationProcess, ct);
    }
}