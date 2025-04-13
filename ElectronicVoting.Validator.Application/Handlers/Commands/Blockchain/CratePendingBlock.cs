using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.Repository;
using MediatR;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;

public record CratePendingBlock : IRequest;

public class CratePendingBlockHandler:IRequestHandler<CratePendingBlock>
{
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IBlockRepository _blockRepository;
    private readonly IPendingBlockRepository _pendingBlockRepository;
    private readonly IPBFTSequenceRepository _pbftSequenceRepository;
    private readonly IPendingTransactionRepository _pendingTransactionRepository;

    public CratePendingBlockHandler(IPendingTransactionRepository pendingTransactionRepository, IPendingBlockRepository pendingBlockRepository, IUnitOfWork unitOfWork, IPBFTSequenceRepository pbftSequenceRepository, IBlockRepository blockRepository, IMediator mediator)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _pbftSequenceRepository = pbftSequenceRepository;
        _blockRepository = blockRepository;
        _pendingBlockRepository = pendingBlockRepository;
        _pendingTransactionRepository = pendingTransactionRepository;
    }

    public async Task Handle(CratePendingBlock request, CancellationToken cancellationToken)
    {
        Console.WriteLine("CratePendingBlock");
        var isLastBlockProcessed = await _pendingBlockRepository.IsLastBlockProcessed(cancellationToken);
        var unprocessedPendingTransactions = await _pendingTransactionRepository.GetUnprocessedTransactionsAsync(cancellationToken);
        
        if(ShouldCreateNewBlock(unprocessedPendingTransactions, isLastBlockProcessed))
            return;

        try
        {
            await _unitOfWork.BeginTransaction(cancellationToken);
            var pendingBlockId = await CreateAndSaveBlock(unprocessedPendingTransactions, cancellationToken);
            await _unitOfWork.Commit(cancellationToken);

            await _mediator.Send(new BroadcastPendingBlock(pendingBlockId), cancellationToken);
        }
        catch (Exception e)
        {
            await _unitOfWork.Rollback(cancellationToken);
        }
    }

    private async Task<long> CreateAndSaveBlock(IEnumerable<PendingTransaction> unprocessedTransactions, CancellationToken cancellationToken)
    {
        var sequenceNumber = await CreateAndSaveSequence(cancellationToken);
        var lastBlock = await _blockRepository.GetLastBlock(cancellationToken);
        
        foreach (var pendingTransaction in unprocessedTransactions)
            pendingTransaction.IsProcessed = true;
        
        var pendingBlockId = await _pendingBlockRepository.GetLastIdAsync(cancellationToken);
        var pendingBlock = new PendingBlock()
        {
            Id = pendingBlockId + 1,
            CreatedDate = DateTime.Now,
            PreviousHash = lastBlock.Hash,
            PbftSequenceNumber = sequenceNumber,
            PendingTransactions = unprocessedTransactions.ToList()
        };
        
        pendingBlock.Hash = HashCalculator.ComputeHash(pendingBlock);
        await _pendingBlockRepository.AddAsync(pendingBlock, cancellationToken);
        return pendingBlock.Id;
    }
    
    private async Task<long> CreateAndSaveSequence(CancellationToken cancellationToken)
    {
        var lastSequenceNumber = await _pbftSequenceRepository.GetLastSequenceNumberAsync(cancellationToken);
        var newSequence = new PBFTSequence()
        {
            SequenceNumber = lastSequenceNumber + 1
        };
        await _pbftSequenceRepository.AddAsync(newSequence, cancellationToken);

        return newSequence.SequenceNumber;
    }
    
    private static bool ShouldCreateNewBlock(IEnumerable<PendingTransaction> unprocessedTransactions, bool isLastBlockConfirmed)
    {
        return unprocessedTransactions.Any() && (isLastBlockConfirmed);
    }
}