using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Domain.Models.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public class StartLocalVoteValidationCommand : ISignedCommand, ITransaction
{
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
    
    public Guid VoteEncryptionId { get; set; }
    public VoteEncryption VoteEncryption { get; set; }
    public Guid VoteValidationProcessId { get; set; }
    public DateTime StartedAt { get; set; }
    public DateTime FinishedAt { get; set; }
}

[WolverineHandler]
public class StartLocalVoteValidationHandler(
    IElectionValidatorService electionValidatorService,
    IVoteValidationProcessRepository voteValidationProcessRepository,
    IVoteValidationService voteValidationService,
    IVoteValidationResultRepository voteValidationResultRepository,
    ISignatureService signatureService,
    IValidationVoteApiService validationVoteApiService,
    ILocalVoteValidationProcessRepository localVoteValidationProcessRepository,
    IVoteEncryptionRepository voteEncryptionRepository)
{
    public async Task HandleAsync(StartLocalVoteValidationCommand command, bool isValidSigned, CancellationToken ct)
    {
        if (!isValidSigned)
            return;
        
        if (await IsNotOrganizerValidatedVote(command.SignedByValidatorId, ct))
        {
            await CreateVoteEncryption(command, ct);
            var resultCreateVoteValidationProcess = await CreateVoteValidationProcess(command, ct);
            if (!resultCreateVoteValidationProcess)
                return;
        }
        var localVoteValidatedProcessId = await CreateAndSaveLocalVoteValidatedProcess(command, ct);
        
        var voteValidationResult = await CreateAndSaveVoteValidationResult(command, localVoteValidatedProcessId, ct);
        var receiveVoteValidationCommand = await CreateAndSignReceiveVoteValidation(voteValidationResult, ct);
        await validationVoteApiService.BroadcastReceiveLocalVoteValidationAsync(receiveVoteValidationCommand, ct);
    }
    
    private async Task<bool> IsNotOrganizerValidatedVote(Guid signedByValidatorId, CancellationToken ct)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        return currentValidatorId != signedByValidatorId;
    }

    private async Task CreateVoteEncryption(StartLocalVoteValidationCommand command, CancellationToken ct)
    {
        VoteEncryptionEntity voteEncryptionEntity = new()
        {
            Id = command.VoteEncryptionId,
            VoteEncryption = command.VoteEncryption,
        };
        
        await voteEncryptionRepository.AddAsync(voteEncryptionEntity, ct);
    }
    
    private async Task<bool> CreateVoteValidationProcess(StartLocalVoteValidationCommand command, CancellationToken ct)
    {
        var isExistValidationProcess = await voteValidationProcessRepository.ExistByIdAsync(command.VoteValidationProcessId, ct);
        if (isExistValidationProcess)
            return false;
        
        var voteValidationProcessEntity = CreateVoteValidationProcessEntity(command);
        await voteValidationProcessRepository.AddAsync(voteValidationProcessEntity, ct);
        return true;
    }

    private static VoteValidationProcessEntity CreateVoteValidationProcessEntity(StartLocalVoteValidationCommand command)
    {
        return new VoteValidationProcessEntity
        {
            Id = command.VoteValidationProcessId,
            Status = VoteValidationProcessStatus.InProgress,
            VoteEncryptionId = command.VoteEncryptionId,
            StartedAt = command.StartedAt,
            FinishedAt = command.FinishedAt,
        };
    }

    
    private async Task<VoteValidationResultDto> CreateAndSaveVoteValidationResult(StartLocalVoteValidationCommand command, Guid  localVoteValidationProcessId, CancellationToken ct)
    {
        var validationStartedAt = DateTime.UtcNow;
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var voteValidationResult = await voteValidationService.Validate(command.VoteEncryptionId);
        
        VoteValidationResultEntity voteValidationResultEntity = new()
        {
            Id = Guid.NewGuid(),
            IsValid = voteValidationResult,
            ValidatorId = currentValidatorId,
            VoteEncryptionId = command.VoteEncryptionId,
            LocalVoteValidationProcessId = localVoteValidationProcessId,
            VoteValidationProcessId = command.VoteValidationProcessId,
        };
        await voteValidationResultRepository.AddAsync(voteValidationResultEntity, ct);
        
        return new VoteValidationResultDto()
        {
            IsValid = voteValidationResult,
            VoteEncryptionId = command.VoteEncryptionId,
            VoteValidationProcessId = voteValidationResultEntity.VoteValidationProcessId,
            StartedAt = validationStartedAt,
            FinishedAt = DateTime.UtcNow,
        };
    }

    private async Task<ReceiveLocalVoteValidationCommand> CreateAndSignReceiveVoteValidation(VoteValidationResultDto voteValidationResult, CancellationToken ct)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        ReceiveLocalVoteValidationCommand receiveLocalVoteValidationCommand = new()
        {
            SignedByValidatorId  = currentValidatorId,
            VoteValidationResult = voteValidationResult,
        };
        
        var signature = signatureService.Sign(receiveLocalVoteValidationCommand);
        receiveLocalVoteValidationCommand.Signature = signature;
        return receiveLocalVoteValidationCommand;
    }

    private async Task<Guid> CreateAndSaveLocalVoteValidatedProcess(StartLocalVoteValidationCommand command, CancellationToken ct)
    {
        LocalVoteValidationProcessEntity localVoteValidationProcessEntity = new()
        {
            Id = Guid.NewGuid(),
            VoteEncryptionId = command.VoteEncryptionId,
            VoteValidationProcessId = command.VoteValidationProcessId,
            Status = LocalVoteValidationStatus.Registered,
        };
        
        await localVoteValidationProcessRepository.AddAsync(localVoteValidationProcessEntity, ct);
        return localVoteValidationProcessEntity.Id;
    }
}