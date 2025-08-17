namespace ElectronicVoting.Validator.Domain.Interface.Processes;

public interface IVoteValidationTimeoutProcessor
{
    Task ProcessAsync(CancellationToken ct);
}