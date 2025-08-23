using System.Text.Json;
using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Processes;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class StartLocalBlockValidationCommand :ISignedCommand, ITransaction
{
    public Guid PendingBlockId { get; set; }
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }    
}

public class StartLocalBlockValidationHandler(
    IPendingBlockStorageService pendingBlockStorageService,
    IElectionValidatorService electionValidatorService,
    ISignatureService signatureService,
    IBlockValidationApiService blockValidationApiService,
    IBlockValidationResultRepository blockValidationResultRepository,
    IPendingBlockRepository pendingBlockRepository,
    IPbftBlockValidationProcess pbftBlockValidationProcess)
{
    public async Task HandleAsync(StartLocalBlockValidationCommand command, bool isValidSigned, CancellationToken ct)
    {
        Console.WriteLine("StartLocalBlockValidationHandler Start");
        if (!isValidSigned)
            return;
        
        var originalPendingBlockDetailsResult = await pendingBlockStorageService.GetPendingBlockDetailsAsync(command.PendingBlockId, ct);
        await StoreInitialBlockValidationAsync(command, originalPendingBlockDetailsResult.Value, ct);
        await ValidateLocallyAndBroadcastResultAsync(command, originalPendingBlockDetailsResult, ct);
    }
    
    private async Task ValidateLocallyAndBroadcastResultAsync(StartLocalBlockValidationCommand command, Result<PendingBlockDetailsDto> originalPendingBlockDetailsResult, CancellationToken ct)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var reconstructPendingBlockResult = await pbftBlockValidationProcess.ProcessAsync(originalPendingBlockDetailsResult.Value, command.SignedByValidatorId, ct);
        var receiveLocalBlockValidationCommand = CreateReceiveLocalBlockValidationCommand(originalPendingBlockDetailsResult, reconstructPendingBlockResult, currentValidatorId);
        await CreateAndSaveBlockValidationResultAsync(receiveLocalBlockValidationCommand, currentValidatorId, ct);
        await blockValidationApiService.BroadcastReceiveLocalBlockValidationAsync(receiveLocalBlockValidationCommand, ct);
        Console.WriteLine("StartLocalBlockValidationHandler End");
    }
    
    private async Task StoreInitialBlockValidationAsync(StartLocalBlockValidationCommand command, PendingBlockDetailsDto pendingBlockDetails, CancellationToken ct)
    {
        var currentValidatorId = await electionValidatorService.GetCurrentValidatorIdAsync(ct);
        
        await CreateAndSavePendingBlockEntity(command, ct);
        await CreateAndSaveBlockValidationResultForLeader(pendingBlockDetails, command.SignedByValidatorId, ct);
    }
    
    private async Task<BlockValidationResultEntity> CreateAndSaveBlockValidationResultForLeader(PendingBlockDetailsDto originalPendingBlockDetails, Guid leaderValidatorId ,CancellationToken ct)
    {
        var blockValidationResultEntity = BlockValidationResultFactory.CreateForLeader(originalPendingBlockDetails, leaderValidatorId);
        await blockValidationResultRepository.AddAsync(blockValidationResultEntity, ct);
        
        return blockValidationResultEntity;
    }
    
    private async Task<PendingBlockEntity> CreateAndSavePendingBlockEntity(StartLocalBlockValidationCommand command, CancellationToken ct)
    {
        PendingBlockEntity pendingBlockEntity = new()
        {
            Id = command.PendingBlockId,
            Hash = string.Empty,
            Status = PendingBlockStatus.Processed,
        };

        await pendingBlockRepository.AddAsync(pendingBlockEntity, ct);
        return pendingBlockEntity;
    }
    
    private async Task CreateAndSaveBlockValidationResultAsync(ReceiveLocalBlockValidationCommand command, Guid currentValidatorId, CancellationToken ct) 
    {
        var entity = BlockValidationResultFactory.CreateForValidator(command, currentValidatorId);
        await blockValidationResultRepository.AddAsync(entity, ct);
    }
    
    private ReceiveLocalBlockValidationCommand CreateReceiveLocalBlockValidationCommand(Result<PendingBlockDetailsDto> originalPendingBlockDetailsResult, Result<PendingBlockDetailsDto> reconstructPendingBlock, Guid currentValidatorId)
    {
        var isValid = BlockHash.HasValidHash(originalPendingBlockDetailsResult.Value.Hash, reconstructPendingBlock.Value.Hash);
        var command = reconstructPendingBlock.IsFailed
            ? ReceiveLocalBlockValidationCommandFactory.CreateFailedCommand(originalPendingBlockDetailsResult, reconstructPendingBlock, currentValidatorId)
            : ReceiveLocalBlockValidationCommandFactory.CreateSuccessfulCommand(reconstructPendingBlock, isValid, currentValidatorId);
        
        command.Signature = signatureService.Sign(command);
        return command;
    }
}