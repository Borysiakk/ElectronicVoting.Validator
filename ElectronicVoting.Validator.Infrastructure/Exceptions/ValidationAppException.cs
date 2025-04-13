namespace ElectronicVoting.Validator.Infrastructure.Exceptions;

public class ValidationAppException(IReadOnlyDictionary<string, string[]> errors)
    : Exception("One or more validation failures have occurred.")
{
    public IReadOnlyDictionary<string, string[]> Errors { get; } = errors;
}