using ElectronicVoting.Validator.Domain.Entities;
using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework;

public class ValidatorDbContext(DbContextOptions<ValidatorDbContext> options) : DbContext(options)
{
    public DbSet<Node> Nodes { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<PendingBlock> PendingBlocks { get; set; }
    public DbSet<PBFTSequence> PbftSequences { get; set; }
    public DbSet<BlockConfirmation> BlockConfirmations{ get; set; }
    public DbSet<ApplicationSetting> ApplicationSetting { get; set; }
    public DbSet<PendingTransaction> PendingTransactions { get; set; }
    
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new NodeConfiguration());
        modelBuilder.ApplyConfiguration(new PBFTSequenceConfiguration());
        modelBuilder.ApplyConfiguration(new PendingBlockConfiguration());
        modelBuilder.ApplyConfiguration(new BlockConfirmationConfiguration());
        modelBuilder.ApplyConfiguration(new ApplicationSettingConfiguration());
        modelBuilder.ApplyConfiguration(new PendingTransactionConfiguration());
        
    }
}