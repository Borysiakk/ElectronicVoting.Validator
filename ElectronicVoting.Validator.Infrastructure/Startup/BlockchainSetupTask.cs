using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.Repository;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ElectronicVoting.Validator.Infrastructure.Startup;

public class BlockchainSetupTask: IHostedService
{
    private readonly IServiceProvider _serviceProvider;
    public BlockchainSetupTask(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        var blockRepository = scope.ServiceProvider.GetRequiredService<IBlockRepository>();
        var pendBlockRepository = scope.ServiceProvider.GetRequiredService<IPendingBlockRepository>();
        var pbftSequenceRepository = scope.ServiceProvider.GetRequiredService<IPBFTSequenceRepository>();
        Console.WriteLine("StartupTask: Rozpoczynam inicjalizację blockchaina...");
        
        var isExistAnyBlock = await blockRepository.IsAnyBlockExists(cancellationToken);
        if (isExistAnyBlock)
            return;
        
        await unitOfWork.BeginTransaction(cancellationToken);
        var sequence = new PBFTSequence();
        await pbftSequenceRepository.AddAsync(sequence, cancellationToken);
            
        var pendingBlock = CreatePendingBlock();
        await pendBlockRepository.AddAsync(pendingBlock, cancellationToken);
        
        var block = CreateGenesisBlock();
        await blockRepository.AddAsync(block, cancellationToken);

        await unitOfWork.Commit(cancellationToken);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        Console.WriteLine("StartupTask: Kończenie działania.");
        return Task.CompletedTask;
    }
    
    
    private static Block CreateGenesisBlock()
    {
        var block =  new Block
        {
            Id = 0,
            Hash = string.Empty,
            PreviousHash = string.Empty,
            CreatedDate = DateTime.UtcNow,
            PbftSequenceNumber = 0,
            Transactions = new List<Transaction>
            {
                new Transaction
                {
                }
            }
        };
        
        block.Hash = HashCalculator.ComputeHash(block);
        return block;
    }

    private static PendingBlock CreatePendingBlock()
    {
        return new PendingBlock()
        {
            Id = 0,
            Hash = string.Empty,
            PreviousHash = string.Empty,
            PbftSequenceNumber = 0,
            IsProcessed = true,
        };
    }
}