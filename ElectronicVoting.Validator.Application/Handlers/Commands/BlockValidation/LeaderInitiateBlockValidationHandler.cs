using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Microsoft.Extensions.Logging;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public record LeaderInitiateBlockValidationCommand: ITransaction
{
    public PendingBlockDetailsDto PendingBlockDetails { get; set; }
}

[WolverineHandler]
public class LeaderInitiateBlockValidationHandler(
    ISignatureService signatureService,
    IElectionValidatorService electionValidatorService,
    IBlockValidationApiService blockValidationApiService,
    IPendingBlockStorageService pendingBlockStorageService,
    IPendingBlockRepository pendingBlockRepository,
    ILogger<LeaderInitiateBlockValidationHandler> logger,
    IBlockValidationResultRepository blockValidationResultRepository)
{
    public async Task HandleAsync(LeaderInitiateBlockValidationCommand command, CancellationToken ct)
    {
        Console.WriteLine("LeaderInitiateBlockValidationHandler");
        
        var storageResult = await pendingBlockStorageService.StoreGetPendingBlockDetailsAsync(command.PendingBlockDetails, ct);
        if (storageResult.IsFailed)
        {
            logger.LogError("Nie udalo sie zapisac proponowanego bloku!");
            return;
        }
        
        await ProcessPendingBlock(command, ct);
    }
    
    private async Task ProcessPendingBlock(LeaderInitiateBlockValidationCommand command, CancellationToken ct)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var pendingBlockEntity = await CreatePendingBlockEntity(command, ct);
        
        await BroadcastValidationToValidators(pendingBlockEntity.Id, currentValidatorId, ct);
        await UpdateBlockStatusToProcessed(pendingBlockEntity, ct);
        await CreateAndSaveBlockValidationResult(command, currentValidatorId, ct);
    }
    
    private async Task BroadcastValidationToValidators(Guid pendingBlockId, Guid currentValidator, CancellationToken ct)
    {
        var validationCommand = new StartLocalBlockValidationCommand
        {
            PendingBlockId = pendingBlockId,
            SignedByValidatorId = currentValidator,
        };
        
        validationCommand.Signature = signatureService.Sign(validationCommand);
        await blockValidationApiService.BroadcastStartLocalVoteValidationAsync(validationCommand, ct);
    }

    private async Task<PendingBlockEntity> CreatePendingBlockEntity(LeaderInitiateBlockValidationCommand command, CancellationToken ct)
    {
        var pendingBlockEntity = new PendingBlockEntity()
        {
            Id = command.PendingBlockDetails.Id,
            StartedAt = command.PendingBlockDetails.StartedAt,
            FinishedAt = command.PendingBlockDetails.FinishedAt,
            Status = PendingBlockStatus.Created,
            Hash = command.PendingBlockDetails.Hash,
        };
        await pendingBlockRepository.AddAsync(pendingBlockEntity, ct);
        
        return pendingBlockEntity;
    }

    
    private async Task UpdateBlockStatusToProcessed(PendingBlockEntity pendingBlockEntity, CancellationToken ct)
    {
        pendingBlockEntity.Status = PendingBlockStatus.Processed;
        await pendingBlockRepository.UpdateAsync(pendingBlockEntity, ct);
    }
    
    private async Task CreateAndSaveBlockValidationResult(LeaderInitiateBlockValidationCommand command, Guid currentValidatorId, CancellationToken ct)
    {
        var blockValidationResultEntity = BlockValidationResultFactory.CreateForLeader(command.PendingBlockDetails, currentValidatorId);
        await blockValidationResultRepository.AddAsync(blockValidationResultEntity, ct);
    }
}