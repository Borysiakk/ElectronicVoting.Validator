
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Enums;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine.Attributes;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;

public record CheckLocalVoteThresholdReachedCommand: ITransaction
{
    public Guid VoteEncryptionId { get; set; }
    public Guid VoteValidationProcessId { get; set; }
    public Guid LocalVoteValidationProcessId { get; set; }
}

public class CheckLocalVoteThresholdReachedHandler
{
    private readonly ISignatureService _signatureService;
    private readonly IValidationVoteApiService _validationVoteApiService;
    private readonly IElectionValidatorService _electionValidatorService;
    private readonly IValidatorNodeRepository _validatorNodeRepository;
    private readonly IVoteValidationResultRepository _voteValidationResultRepository;
    private readonly ILocalVoteValidationProcessRepository _localVoteValidationProcessRepository;

    public CheckLocalVoteThresholdReachedHandler(IValidatorNodeRepository validatorNodeRepository, IVoteValidationResultRepository voteValidationResultRepository, ILocalVoteValidationProcessRepository localVoteValidationProcessRepository, IValidationVoteApiService validationVoteApiService, IElectionValidatorService electionValidatorService, ISignatureService signatureService)
    {
        _validatorNodeRepository = validatorNodeRepository;
        _voteValidationResultRepository = voteValidationResultRepository;
        _localVoteValidationProcessRepository = localVoteValidationProcessRepository;
        _validationVoteApiService = validationVoteApiService;
        _electionValidatorService = electionValidatorService;
        _signatureService = signatureService;
    }
    
    public async Task HandleAsync(CheckLocalVoteThresholdReachedCommand command, CancellationToken ct)
    {
        var countValidator = await _validatorNodeRepository.CountAsync(ct);
        var countVoteValidationResult = await _voteValidationResultRepository.CountResultsForLocalValidationProcessAsync(command.LocalVoteValidationProcessId, ct);
        if (countVoteValidationResult >= countValidator / 2)
        {
            await ChangeStatusLocalVoteValidationProcess(command.LocalVoteValidationProcessId, ct);
            await NotifyLeaderOfVoteConsensusAsync(command, ct);
        }
    }

    private async Task ChangeStatusLocalVoteValidationProcess(Guid localVoteValidationProcessId, CancellationToken cancellationToken = default)
    {
        var localVoteValidationProcess = await _localVoteValidationProcessRepository.GetByIdAsync(localVoteValidationProcessId, cancellationToken);
        localVoteValidationProcess.Status = LocalVoteValidationStatus.Completed;
        
        await _localVoteValidationProcessRepository.UpdateAsync(localVoteValidationProcess, cancellationToken);
    }

    private async Task NotifyLeaderOfVoteConsensusAsync(CheckLocalVoteThresholdReachedCommand command, CancellationToken cancellationToken = default)
    {
        var currentValidatorId= await _electionValidatorService.GetCurrentValidatorIdAsync(cancellationToken);
        
        LeaderReceiveVoteConsensusReportCommand consensusReportCommand = new()
        {
            SignedByValidatorId = currentValidatorId,
            VoteEncryptionId = command.VoteEncryptionId,
            VoteValidationProcessId = command.VoteValidationProcessId,
        };
        
        consensusReportCommand.Signature = _signatureService.Sign(consensusReportCommand);
        await _validationVoteApiService.BroadcastLeaderReceiveVoteConsensusReportAsync(consensusReportCommand, cancellationToken);
    }

}