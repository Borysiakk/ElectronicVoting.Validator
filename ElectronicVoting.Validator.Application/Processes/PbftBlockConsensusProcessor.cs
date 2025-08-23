using ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Interface.Services;
using ElectronicVoting.Validator.Infrastructure.Configuration;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Microsoft.Extensions.Options;

namespace ElectronicVoting.Validator.Application.Processes;

public interface IPbftBlockConsensusProcessor
{
    public Task ProcessAsync(Guid pendingBlockId, CancellationToken ct);
}

public class PbftBlockConsensusProcessor: IPbftBlockConsensusProcessor
{
    private readonly ConsensusOptions _consensusOptions;
    private readonly ISignatureService _signatureService;
    private readonly IElectionValidatorService _electionValidatorService;
    private readonly IBlockValidationApiService _blockValidationApiService;
    private readonly IBlockValidationResultRepository _blockValidationResultRepository;
    
    public PbftBlockConsensusProcessor(IBlockValidationResultRepository blockValidationResultRepository, IOptions<ConsensusOptions> consensusOptions, ISignatureService signatureService, IElectionValidatorService electionValidatorService, IBlockValidationApiService blockValidationApiService)
    {
        _consensusOptions = consensusOptions.Value;
        _signatureService = signatureService;
        _electionValidatorService = electionValidatorService;
        _blockValidationApiService = blockValidationApiService;
        _blockValidationResultRepository = blockValidationResultRepository;
    }

    public async Task ProcessAsync(Guid pendingBlockId, CancellationToken ct)
    {
        var countByPendingBlock = await _blockValidationResultRepository.CountByPendingBlockId(pendingBlockId, ct);
        if (countByPendingBlock >= _consensusOptions.MinimumValidValidations)
        {
            var receiveBlockConsensusReportCommand = await CreateLeaderReceiveBlockConsensusReportCommand(pendingBlockId, ct);
            await NotifyLeaderOfVoteConsensusAsync(receiveBlockConsensusReportCommand, ct);
        }
    }

    private async Task<LeaderReceiveBlockConsensusReportCommand> CreateLeaderReceiveBlockConsensusReportCommand(Guid pendingBlockId, CancellationToken ct)
    {
        var currentValidatorId = await _electionValidatorService.GetCurrentValidatorIdAsync(ct);
        var command =  new LeaderReceiveBlockConsensusReportCommand()
        {
            PendingBlockId = pendingBlockId,
            SignedByValidatorId = currentValidatorId,
        };
        command.Signature = _signatureService.Sign(command);
        return command;
    }

    private async Task NotifyLeaderOfVoteConsensusAsync(LeaderReceiveBlockConsensusReportCommand command, CancellationToken cancellationToken)
    {
        await _blockValidationApiService.BroadcastLeaderReceiveBlockConsensusReportAsync(command, cancellationToken);
    }
}