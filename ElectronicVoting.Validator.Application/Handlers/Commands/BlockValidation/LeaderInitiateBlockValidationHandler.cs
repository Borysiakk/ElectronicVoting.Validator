using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Interface.Services;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public record LeaderInitiateBlockValidationCommand
{
    public PendingBlockDetailsDto PendingBlockDetails { get; set; }
}

public class LeaderInitiateBlockValidationHandler
{
    private readonly ISignatureService _signatureService;
    private readonly IElectionValidatorService _electionValidatorService;
    private readonly IBlockValidationApiService _blockValidationApiService;
    private readonly IPendingBlockStorageService _pendingBlockStorageService;

    public LeaderInitiateBlockValidationHandler(ISignatureService signatureService,
        IElectionValidatorService electionValidatorService, IBlockValidationApiService blockValidationApiService,
        IPendingBlockStorageService pendingBlockStorageService)
    {
        _signatureService = signatureService;
        _electionValidatorService = electionValidatorService;
        _blockValidationApiService = blockValidationApiService;
        _pendingBlockStorageService = pendingBlockStorageService;
    }

    public async Task HandleAsync(LeaderInitiateBlockValidationCommand command, CancellationToken ct)
    {
        var storageResult = await _pendingBlockStorageService.StoreGetPendingBlockDetailsAsync(command.PendingBlockDetails, ct);
        if (storageResult.IsFailed)
            return;

        await CreateAndBroadcastValidationCommand(command.PendingBlockDetails.Id, ct);

    }
    
    private async Task CreateAndBroadcastValidationCommand(Guid pendingBlockId, CancellationToken ct)
    {
        var currentValidator = await _electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var validationCommand = new StartLocalBlockValidationCommand
        {
            PendingBlockId = pendingBlockId,
            SignedByValidatorId = currentValidator,
        };

        validationCommand.Signature = _signatureService.Sign(validationCommand);
        await _blockValidationApiService.BroadcastStartLocalVoteValidationAsync(validationCommand, ct);
    }
    
}