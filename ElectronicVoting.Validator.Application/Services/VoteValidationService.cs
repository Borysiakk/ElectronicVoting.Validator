namespace ElectronicVoting.Validator.Application.Services;

public interface IVoteValidationService
{
    Task<bool> Validate(Guid id);
}
public class VoteValidationService: IVoteValidationService
{
    public async Task<bool> Validate(Guid id)
    {
        await Task.Delay(100);
        return true;
    }
}