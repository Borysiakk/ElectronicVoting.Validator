using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.DTO.VoteValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using ElectronicVoting.Validator.Infrastructure.Helpers;
using FluentResults;
using Microsoft.Extensions.Logging;

namespace ElectronicVoting.Validator.Application.Factories;

public interface IPendingBlockFactory
{
    Task<Result<PendingBlockDetailsDto>> TryCreatePendingBlockAsync(CancellationToken ct = default);
}
public class PendingBlockFactory: IPendingBlockFactory
{
    private readonly ILogger<PendingBlockFactory> _logger;
    private readonly IVoteValidationProcessRepository _voteValidationProcessRepository;
    private readonly IPendingBlockRepository _pendingBlockRepository;
    private readonly IBlockRepository _blockRepository;
    private readonly IPbftSequenceRepository _pbftSequenceRepository;
    private readonly IElectionValidatorService _electionValidatorService;

    public PendingBlockFactory(IPendingBlockRepository pendingBlockRepository, IBlockRepository blockRepository, IPbftSequenceRepository pbftSequenceRepository, IVoteValidationProcessRepository voteValidationProcessRepository, ILogger<PendingBlockFactory> logger, IVoteEncryptionRepository voteEncryptionRepository, IElectionValidatorService electionValidatorService)
    {
        _pendingBlockRepository = pendingBlockRepository;
        _blockRepository = blockRepository;
        _pbftSequenceRepository = pbftSequenceRepository;
        _voteValidationProcessRepository = voteValidationProcessRepository;
        _electionValidatorService = electionValidatorService;
        _logger = logger;
    }

    public async Task<Result<PendingBlockDetailsDto>> TryCreatePendingBlockAsync(CancellationToken ct = default)
    {
        var isAnyPendingBlockInProcess = await _pendingBlockRepository.ExistsAnyPendingBlockInProcessAsync(ct);
        if (isAnyPendingBlockInProcess)
        {
            _logger.LogWarning("There is already a pending block in process");
            return Result.Fail("There is already a pending block in process");   
        }
        
        var voteValidationsReadyToCommit= await _voteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, ct);
        if (!voteValidationsReadyToCommit.Any())
        {
            _logger.LogWarning("There is no vote validation ready to commit");
            return Result.Fail("There is no vote validation ready to commit");   
        }
        
        return Result.Ok(await CreatePendingBlockDetails(ct));
    }
    
    private async Task<List<VoteValidationProcessEntity>> GetVoteValidationProcessWhenReadyToCommitAsync(CancellationToken ct)
    {
        var votesValidationProcess = await _voteValidationProcessRepository.GetByStatusAsync(VoteValidationProcessStatus.ReadyToCommit, ct);

        foreach (var voteValidationProcess in votesValidationProcess)
            voteValidationProcess.Status = VoteValidationProcessStatus.ProcessedToCommit;
         
        await _voteValidationProcessRepository.UpdateAsync(votesValidationProcess.ToArray(), ct);
        
        return votesValidationProcess;
    }
    
    private async Task<List<PendingTransactionDetailsDto>> CreatePendingTransactionsAsync(CancellationToken ct)
    {
        var votesProcessValidation = await GetVoteValidationProcessWhenReadyToCommitAsync(ct);
        if (!votesProcessValidation.Any())
            return new List<PendingTransactionDetailsDto>();
        
        return votesProcessValidation.Select(a => new PendingTransactionDetailsDto()
        {
            Id = Guid.NewGuid(),
            VoteEncryption = new VoteEncryptionDto()
            {
                Id = a.VoteEncryptionId,
                VoteEncryption = a.VoteEncryption.VoteEncryption,
            },
            VoteValidationProcess = new VoteValidationProcessDto()
            {
                Id = a.Id,
                Status = a.Status,
                StartedAt = a.StartedAt,
                FinishedAt = a.FinishedAt,
                VoteEncryptionId = a.VoteEncryptionId,
            }
        }).ToList();
    }

    private async Task<PendingBlockDetailsDto> CreatePendingBlockDetails(CancellationToken ct = default)
    {
        var pendingTransactionDetails= await CreatePendingTransactionsAsync(ct);
        
        var lastBlock = await _blockRepository.GetLastBlock(ct);
        var lastSequence = await _pbftSequenceRepository.GetLastSequenceNumberAsync(ct);
        var currentValidator = await _electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var pbftSequence = new PbftSequenceDto()
        {
            SequenceNumber = lastSequence + 1,
        };
        
        
        var pendingBlockDetails = new PendingBlockDetailsDto()
        {
            Id = Guid.NewGuid(),
            PbftSequence = pbftSequence,
            PreviousHash = lastBlock == null ? "" : lastBlock.Hash,
            ValidatorNodeId = currentValidator,
            PendingTransactions = pendingTransactionDetails,
        };

        pendingBlockDetails.Hash = BlockHashCalculator.ComputeBlockHash(pendingBlockDetails);
        
        return pendingBlockDetails;
    }
}