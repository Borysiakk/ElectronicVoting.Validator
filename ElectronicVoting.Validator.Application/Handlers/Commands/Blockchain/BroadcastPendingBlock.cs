using ElectronicVoting.Validator.Application.Service;
using ElectronicVoting.Validator.Domain.Models.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Repository;
using MediatR;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;

public record BroadcastPendingBlock(long PendingBlockId) : IRequest
{
    public long PendingBlockId { get; set; } = PendingBlockId;
}

public class BroadcastPendingBlockHandler: IRequestHandler<BroadcastPendingBlock>
{
    private readonly IPendingBlockRepository _pendingBlockRepository;
    private readonly IElectionConsensusService _electionConsensusService;

    public BroadcastPendingBlockHandler(IPendingBlockRepository pendingBlockRepository, IElectionConsensusService electionConsensusService)
    {
        _pendingBlockRepository = pendingBlockRepository;
        _electionConsensusService = electionConsensusService;
    }

    public async Task Handle(BroadcastPendingBlock request, CancellationToken cancellationToken)
    {
        var pendingBlockDto = await PrepareBlockForValidation(request.PendingBlockId, cancellationToken);
        await _electionConsensusService.ExecutePrePrepareAsync(pendingBlockDto, cancellationToken);
    }
    
    private async Task<PendingBlockDto> PrepareBlockForValidation(long pendingBlockId, CancellationToken cancellationToken)
    {
        var block = await _pendingBlockRepository.GetByIdAsync(pendingBlockId, cancellationToken);
        var pendingTransactionDtos = block.PendingTransactions.Select(tx => new PendingTransactionDto()
        {
            Id = tx.Id,
            Data = tx.Data,
        }).ToList();
        
        return new PendingBlockDto()
        {
            Id = block.Id,
            Hash = block.Hash,
            PbftSequenceNumber = block.PbftSequenceNumber,
            PreviousHash = block.PreviousHash ?? string.Empty,
            PendingTransaction = pendingTransactionDtos,
        };
    }
}