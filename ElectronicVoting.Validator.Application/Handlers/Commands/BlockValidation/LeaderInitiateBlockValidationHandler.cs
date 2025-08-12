using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Commands;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Domain.Models.BlockValidation;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;


public class LeaderInitiateBlockValidationHandler
{
    private readonly ISignatureService _signatureService;
    private readonly IElectionValidatorService _electionValidatorService;
    private readonly IBlockValidationApiService _blockValidationApiService;
    public LeaderInitiateBlockValidationHandler(ISignatureService signatureService, IElectionValidatorService electionValidatorService, IBlockValidationApiService blockValidationApiService)
    {
        _signatureService = signatureService;
        _electionValidatorService = electionValidatorService;
        _blockValidationApiService = blockValidationApiService;
    }

    public async Task HandleAsync(LeaderInitiateBlockValidationCommand command, CancellationToken ct)
    {
        var startLocalBlockValidationCommand = await CreateAndSignStartLocalBlockValidationCommand(command.PendingBlock, ct);
        await _blockValidationApiService.BroadcastStartLocalVoteValidationAsync(startLocalBlockValidationCommand, ct);
    }
    
    private async Task<StartLocalBlockValidationCommand> CreateAndSignStartLocalBlockValidationCommand(PendingBlock pendingBlock, CancellationToken cancellationToken = default)
    {
        var currentValidatorId= await _electionValidatorService.GetCurrentValidatorIdAsync(cancellationToken);
        StartLocalBlockValidationCommand startLocalBlockValidationCommand = new()
        {
            PendingBlock = pendingBlock,
            SignedByValidatorId = currentValidatorId,
        };
        
        var signature = _signatureService.Sign(startLocalBlockValidationCommand);
        startLocalBlockValidationCommand.Signature = signature;
        
        return startLocalBlockValidationCommand;
    }
}