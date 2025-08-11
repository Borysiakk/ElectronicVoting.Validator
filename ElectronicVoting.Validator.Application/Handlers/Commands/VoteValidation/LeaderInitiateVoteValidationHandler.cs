using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public record LeaderInitiateVoteValidationCommand: ITransaction
{
    public Guid VoteEncryptionId { get; set; }
}

[WolverineHandler]
public class LeaderInitiateVoteValidationHandler(
    IVoteEncryptionRepository voteEncryptionRepository,
    ILogger<LeaderInitiateVoteValidationHandler> logger,
    IVoteValidationProcessRepository voteValidationProcessRepository,
    ISignatureService signatureService,
    IElectionValidatorService electionValidatorService,
    IValidationVoteApiService validationVoteApiService)
{
    private const string VoteEncryptionNotFound = "Vote encryption not found";

    public async Task HandleAsync(LeaderInitiateVoteValidationCommand command, CancellationToken cancellationToken)
    {
        var voteEncryption = await voteEncryptionRepository.GetByIdAsync(command.VoteEncryptionId, cancellationToken);
        if (voteEncryption is null)
        {
            logger.LogError(VoteEncryptionNotFound + $"{command.VoteEncryptionId}");
            return;
        }
        
        var voteValidationProcess = await CreateVoteValidationProcess(voteEncryption, cancellationToken);
        await NotifyValidatorsAboutVoteValidation(voteEncryption, voteValidationProcess, cancellationToken);
    }
    
    private async Task<VoteValidationProcessEntity> CreateVoteValidationProcess(VoteEncryptionEntity voteEncryptionEntity, CancellationToken cancellationToken = default)
    {
        VoteValidationProcessEntity voteValidationProcessEntity = new()
        {
            Id = Guid.NewGuid(),
            Status = VoteValidationProcessStatus.Registered,
            VoteEncryptionId = voteEncryptionEntity.Id,
            StartedAt = DateTime.UtcNow,
            FinishedAt = DateTime.UtcNow.AddMinutes(10)
        };
        return await voteValidationProcessRepository.AddAsync(voteValidationProcessEntity, cancellationToken);
    }

    private async Task NotifyValidatorsAboutVoteValidation(VoteEncryptionEntity voteEncryptionDocument, VoteValidationProcessEntity voteValidationProcess, CancellationToken cancellationToken)
    {
        var currentValidatorId= await electionValidatorService.GetCurrentValidatorIdAsync(cancellationToken);
        StartLocalVoteValidationCommand command = new()
        {
            SignedByValidatorId = currentValidatorId,
            VoteEncryptionId = voteEncryptionDocument.Id,
            VoteEncryption = voteEncryptionDocument.VoteEncryption,
            VoteValidationProcessId = voteValidationProcess.Id,
            StartedAt = voteValidationProcess.StartedAt,
            FinishedAt = voteValidationProcess.FinishedAt,
        };
        command.Signature = signatureService.Sign(command);
        await validationVoteApiService.BroadcastStartLocalVoteValidationAsync(command, cancellationToken);
        
        voteValidationProcess.Status = VoteValidationProcessStatus.InProgress;
        await voteValidationProcessRepository.UpdateAsync(voteValidationProcess, cancellationToken);
    }
}