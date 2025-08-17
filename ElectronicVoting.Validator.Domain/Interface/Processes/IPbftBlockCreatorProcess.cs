namespace ElectronicVoting.Validator.Domain.Interface.Processes;

public interface IPbftBlockCreatorProcess
{
    Task ProcessAsync(CancellationToken ct);
}