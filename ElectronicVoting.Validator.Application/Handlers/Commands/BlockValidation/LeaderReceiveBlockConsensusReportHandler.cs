

using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.BlockValidation;

public class LeaderReceiveBlockConsensusReportCommand: ISignedCommand, ITransaction
{
    public Guid PendingBlockId { get; set; }
    public string Signature { get; set; }
    public Guid SignedByValidatorId { get; set; }
}

public class LeaderReceiveBlockConsensusReportHandler
{
    public async Task HandleAsync(LeaderReceiveBlockConsensusReportCommand command, CancellationToken ct)
    {
        Console.WriteLine("");
    }
}