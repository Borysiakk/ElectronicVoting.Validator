using ElectronicVoting.Validator.Domain.Entities;
using ElectronicVoting.Validator.Domain.Entities.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Repositories;
using Microsoft.Extensions.Configuration;
using static System.String;

namespace ElectronicVoting.Validator.Application.Services;

public interface IElectionValidatorService
{
    public Task<Guid> GetCurrentValidatorIdAsync(CancellationToken cancellationToken);
    public Task<IEnumerable<ValidatorNode>> GetAllValidatorsExceptCurrent(CancellationToken cancellationToken);
}

public class ElectionValidatorService(IConfiguration configuration, IValidatorNodeRepository validatorNodeRepository) : IElectionValidatorService
{
    public async Task<Guid> GetCurrentValidatorIdAsync(CancellationToken cancellationToken)
    {
        var name = configuration["Validator:Name"];
        if(IsNullOrEmpty(name))
            throw new Exception("Nie udalo sie pobrac nazwy validatora.");

        var validator = await validatorNodeRepository.GetByName(name, cancellationToken);
        
        if(validator == null)
            throw new Exception("Nie znaleziono validatora.");
        
        return validator.Id;
    }

    public async Task<IEnumerable<ValidatorNode>> GetAllValidatorsExceptCurrent(CancellationToken cancellationToken)
    {
        var name = configuration["Validator:Name"];
        if(IsNullOrEmpty(name))
            throw new Exception("Nie udalo sie pobrac nazwy validatora.");
        
        return await validatorNodeRepository.GetAllExceptAsync(name, cancellationToken);
    }
}