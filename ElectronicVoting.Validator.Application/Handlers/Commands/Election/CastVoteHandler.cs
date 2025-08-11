using ElectronicVoting.Validator.Application.Handlers.Commands.VoteValidation;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Models.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;
using Wolverine;

namespace ElectronicVoting.Validator.Application.Handlers.Commands.Election;

public record CastVoteCommand: ITransaction
{
    public Guid Id { get; set; }
    public VoteEncryption VoteEncryption { get; set; }
}

public class CastVoteHandler(IVoteEncryptionRepository voteEncryptionRepository, IMessageBus bus)
{
    public async Task Handle(CastVoteCommand command, CancellationToken cancellationToken)
    {
        await CreateAndSaveVoteEncryption(command, cancellationToken);
        await bus.SendAsync(new LeaderInitiateVoteValidationCommand()
        {
            VoteEncryptionId = command.Id
        });
    }
    
    private async Task CreateAndSaveVoteEncryption(CastVoteCommand command, CancellationToken cancellationToken)
    {
        var voteEncryptionEntity = new VoteEncryptionEntity
        {
            Id = command.Id,
            VoteEncryption = command.VoteEncryption
        };
        
        await voteEncryptionRepository.AddAsync(voteEncryptionEntity, cancellationToken);
    }
}