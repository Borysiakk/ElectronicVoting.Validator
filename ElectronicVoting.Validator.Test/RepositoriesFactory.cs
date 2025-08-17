using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

namespace ElectronicVoting.Validator.Test;

public class RepositoriesFactory
{
    public IVoteEncryptionRepository VoteEncryptionRepository { get;}
    public IVoteValidationProcessRepository VoteValidationProcessRepository { get;}
    public IVoteConsensusConfirmationRepository VoteConsensusConfirmationRepository { get;}
    public ILocalVoteValidationProcessRepository LocalVoteValidationProcessRepository { get;}
    
    public RepositoriesFactory(ValidatorLedgerDbContext validatorLedgerDbContext)
    {
        VoteEncryptionRepository = new VoteEncryptionRepository(validatorLedgerDbContext);
        VoteValidationProcessRepository = new VoteValidationProcessRepository(validatorLedgerDbContext);
        VoteConsensusConfirmationRepository = new VoteConsensusConfirmationRepository(validatorLedgerDbContext);
        LocalVoteValidationProcessRepository = new LocalVoteValidationProcessRepository(validatorLedgerDbContext);
    }
}