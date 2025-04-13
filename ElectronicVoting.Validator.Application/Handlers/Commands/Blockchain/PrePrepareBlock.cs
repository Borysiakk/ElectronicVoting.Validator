using ElectronicVoting.Validator.Application.Service;
using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Domain.Enum;
using ElectronicVoting.Validator.Domain.Models.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Repository;
using MediatR;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;

public class PrePrepareBlock: IRequest
{
    public PendingBlockDto PendingBlockDto { get; set; }
}

public class PrePrepareBlockHandler: IRequestHandler<PrePrepareBlock>
{
    private readonly INodeRepository _nodeRepository;
    private readonly IPBFTSequenceRepository _sequenceRepository;
    private readonly IPendingBlockRepository _pendingBlockRepository;
    private readonly IElectionConsensusService _electionConsensusService;
    private readonly IBlockConfirmationRepository _blockConfirmationRepository;
    
    public PrePrepareBlockHandler(IPendingBlockRepository pendingBlockRepository, IPBFTSequenceRepository sequenceRepository, INodeRepository nodeRepository, IBlockConfirmationRepository blockConfirmationRepository, IElectionConsensusService electionConsensusService)
    {
        _nodeRepository = nodeRepository;
        _sequenceRepository = sequenceRepository;
        _pendingBlockRepository = pendingBlockRepository;
        _blockConfirmationRepository = blockConfirmationRepository;
        _electionConsensusService = electionConsensusService;
    }

    public async Task Handle(PrePrepareBlock request, CancellationToken cancellationToken)
    {
        Console.WriteLine("PrePrepareBlock");
        var pendingBlockEntity = await SavePendingBlock(request.PendingBlockDto, cancellationToken);
        var resultValidateBlock = await ValidateBlock(pendingBlockEntity, cancellationToken);
        await SaveValidationResultForBlock(pendingBlockEntity, cancellationToken);

        var prepareBlock = new PrepareBlock(pendingBlockEntity.Id);
        await _electionConsensusService.ExecutePrepareAsync(prepareBlock, cancellationToken);
    }

    private async Task<PendingBlock> SavePendingBlock(PendingBlockDto pendingBlockDto, CancellationToken cancellationToken)
    {
        var pendingBlock = new PendingBlock()
        {
            Id = pendingBlockDto.Id,
            Hash = pendingBlockDto.Hash,
            PreviousHash = pendingBlockDto.PreviousHash,
            PendingTransactions = pendingBlockDto.PendingTransaction.Select(a => new PendingTransaction()
            {
                Id = a.Id,
                Data = a.Data,
            }).ToList(),
        };
        var pendingBlockEntity = await _pendingBlockRepository.AddAsync(pendingBlock, cancellationToken);
        
        return pendingBlockEntity;
    }

    private async Task<bool> ValidateBlock(PendingBlock pendingBlock, CancellationToken cancellationToken)
    {
        var lastSequence = await _sequenceRepository.GetLastSequenceAsync(cancellationToken);
        if(pendingBlock.PbftSequenceNumber != lastSequence.SequenceNumber +1)
            return false;
        
        var blocHash = HashCalculator.ComputeHash(pendingBlock);
        
        return blocHash == pendingBlock.Hash;
    }

    private async Task SaveValidationResultForBlock(PendingBlock pendingBlock, CancellationToken ct)
    {
        var node = await _nodeRepository.FindMyNodeAsync(ct);
        var blockConfirmation = new BlockConfirmation()
        {
            Type = ConfirmationType.Prepare,
            PendingBlockId = pendingBlock.Id,
        };
        
        await _blockConfirmationRepository.AddAsync(blockConfirmation, ct);
    }
}
