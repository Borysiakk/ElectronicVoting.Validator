using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger;

public class ValidatorLedgerDbContext(DbContextOptions<ValidatorLedgerDbContext> options) : DbContext(options)
{
    public DbSet<VoteEncryptionEntity> VoteEncryption { get; set; }
    public DbSet<VoteConsensusConfirmationEntity> VoteConsensusConfirmation { get; set; }
    public DbSet<LocalVoteValidationProcessEntity> LocalVoteValidationProcesses { get; set; }
    public DbSet<VoteValidationProcessEntity> VoteValidationProcesses { get; set; }
    public DbSet<VoteValidationResultEntity> VoteValidationResults { get; set; }

    public DbSet<BlockEntity> Blocks { get; set; }
    public DbSet<TransactionEntity> Transactions { get; set; }
    public DbSet<PbftSequenceEntity> PbftSequences { get; set; }
    
    public DbSet<PendingBlockEntity> PendingBlocks { get; set; }
    public DbSet<BlockValidationResultEntity> BlockValidationResults { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new VoteEncryptionConfiguration());
        modelBuilder.ApplyConfiguration(new VoteConsensusConfirmationConfiguration());
        modelBuilder.ApplyConfiguration(new LocalVoteValidationProcessConfiguration());
        modelBuilder.ApplyConfiguration(new VoteValidationProcessConfiguration());
        modelBuilder.ApplyConfiguration(new VoteValidationResultConfiguration());
        
        modelBuilder.ApplyConfiguration(new PendingBlockConfiguration());
        modelBuilder.ApplyConfiguration(new BlockConfiguration());
        modelBuilder.ApplyConfiguration(new TransactionConfiguration());
        modelBuilder.ApplyConfiguration(new PbftSequenceConfiguration());
        modelBuilder.ApplyConfiguration(new BlockValidationResultConfiguration());
    }
}