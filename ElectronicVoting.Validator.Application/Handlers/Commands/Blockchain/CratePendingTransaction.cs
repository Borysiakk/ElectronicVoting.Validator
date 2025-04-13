using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.Repository;
using Hangfire;
using MediatR;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Blockchain;

public record CratePendingTransaction: IRequest
{
    
}

public class CratePendingTransactionHandler: IRequestHandler<CratePendingTransaction>
{
    private const string TransactionThresholdForBlock = "TransactionThresholdForBlock";
    
    private readonly IMediator _mediator;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IApplicationSettingRepository _applicationSettingRepository;
    private readonly IPendingTransactionRepository _pendingTransactionRepository;
    
    public CratePendingTransactionHandler(IApplicationSettingRepository applicationSettingRepository, IPendingTransactionRepository pendingTransactionRepository, IUnitOfWork unitOfWork, IMediator mediator)
    {
        _mediator = mediator;
        _unitOfWork = unitOfWork;
        _applicationSettingRepository = applicationSettingRepository;
        _pendingTransactionRepository = pendingTransactionRepository;
    }

    [Queue("Queue.PendingTransaction")]
    public async Task Handle(CratePendingTransaction request, CancellationToken cancellationToken)
    {
        await CreateAndSavePendingTransaction(cancellationToken);
        await CreateBlockWhenThresholdExceeded(cancellationToken);
    }
    
    private async Task CreateAndSavePendingTransaction(CancellationToken cancellationToken)
    {
        var lastId = await _pendingTransactionRepository.GetLastIdAsync(cancellationToken);
        var pendingTransaction = new PendingTransaction()
        {
            Id = lastId + 1,
            IsProcessed = false,
            Data = "Test Transaction 1.0",
        };
        
        await _pendingTransactionRepository.AddAsync(pendingTransaction, cancellationToken);
    }

    private async Task CreateBlockWhenThresholdExceeded(CancellationToken cancellationToken)
    {
        var transactionThresholdForBlock = await _applicationSettingRepository.GetByKeyAsync<int>(TransactionThresholdForBlock, cancellationToken);
        if (transactionThresholdForBlock is null)
            throw new Exception("");
        
        var unprocessedTransactionCount = await _pendingTransactionRepository.GetUnprocessedTransactionCountAsync(cancellationToken);

        if (unprocessedTransactionCount >= transactionThresholdForBlock)
            await _mediator.Send(new CratePendingBlock(), cancellationToken);
    }
}