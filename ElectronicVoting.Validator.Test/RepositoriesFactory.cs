using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Repositories;

namespace ElectronicVoting.Validator.Test;

public class RepositoriesFactory
{
    
    public IPendingBlockRepository PendingBlockRepository { get; }
    public IBlockRepository BlockRepository { get; }
    public IPbftSequenceRepository PbftSequenceRepository { get; }
    public IVoteEncryptionRepository VoteEncryptionRepository { get;}
    public IVoteValidationResultRepository VoteValidationResultRepository { get;}
    public IVoteValidationProcessRepository VoteValidationProcessRepository { get;}
    public IVoteConsensusConfirmationRepository VoteConsensusConfirmationRepository { get;}
    public ILocalVoteValidationProcessRepository LocalVoteValidationProcessRepository { get;}
    
    public RepositoriesFactory(ValidatorLedgerDbContext validatorLedgerDbContext)
    {
        BlockRepository = new BlockRepository(validatorLedgerDbContext);
        PbftSequenceRepository = new PbftSequenceRepository(validatorLedgerDbContext);
        PendingBlockRepository = new PendingBlockRepository(validatorLedgerDbContext);
        VoteEncryptionRepository = new VoteEncryptionRepository(validatorLedgerDbContext);
        VoteValidationResultRepository = new VoteValidationResultRepository(validatorLedgerDbContext);
        VoteValidationProcessRepository = new VoteValidationProcessRepository(validatorLedgerDbContext);
        VoteConsensusConfirmationRepository = new VoteConsensusConfirmationRepository(validatorLedgerDbContext);
        LocalVoteValidationProcessRepository = new LocalVoteValidationProcessRepository(validatorLedgerDbContext);
        
        
    }
}