using ElectronicVoting.Validator.Application.Factories;
using ElectronicVoting.Validator.Application.Processes;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Microsoft.Extensions.Logging;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class ReceiveLocalBlockValidationCommand: ISignedCommand, ITransaction
{
    public bool IsValid { get; set; }
    public string Hash { get; set; }
    public Guid PendingBlockId { get; set; }
    public List<string> RejectionReason { get; set; }
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
}

public class ReceiveLocalBlockValidationHandler
{
    private readonly ILogger<ReceiveLocalBlockValidationHandler> _logger;
    private readonly PbftBlockConsensusProcessor _pbftBlockConsensusProcessor;
    private readonly IBlockValidationResultRepository _blockValidationResultRepository;
    
    public ReceiveLocalBlockValidationHandler(ILogger<ReceiveLocalBlockValidationHandler> logger, IBlockValidationResultRepository blockValidationResultRepository, PbftBlockConsensusProcessor pbftBlockConsensusProcessor)
    {
        _logger = logger;
        _pbftBlockConsensusProcessor = pbftBlockConsensusProcessor;
        _blockValidationResultRepository = blockValidationResultRepository;
    }

    public async Task HandleAsync(ReceiveLocalBlockValidationCommand command, bool isValidSigned, CancellationToken ct)
    {
        Console.WriteLine("ReceiveLocalBlockValidationHandler");
        
        if (!isValidSigned)
            return;
        
        await CreateAndSaveBlockValidationResult(command, ct);
        await _pbftBlockConsensusProcessor.ProcessAsync(command.PendingBlockId, ct);
    }

    private async Task CreateAndSaveBlockValidationResult(ReceiveLocalBlockValidationCommand command, CancellationToken ct)
    {
        var blockValidationResultEntity = BlockValidationResultFactory.CreateForValidator(command, command.SignedByValidatorId);
        await _blockValidationResultRepository.AddAsync(blockValidationResultEntity, ct);
    }
}