using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class PendingBlockConfiguration: IEntityTypeConfiguration<PendingBlockEntity>
{
    public void Configure(EntityTypeBuilder<PendingBlockEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        
        builder.Property(x => x.Hash)
            .IsRequired()
            .HasMaxLength(256);
        
        builder.Property(a=>a.StartedAt).IsRequired();
        builder.Property(a=>a.FinishedAt).IsRequired(false);
        
        builder.Property(x => x.Status).HasConversion<string>();
        
        builder.HasMany(x => x.PendingTransactions)
            .WithOne(x => x.PendingBlock)
            .HasForeignKey(x => x.PendingBlockId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasOne(x => x.PbftSequence)
            .WithOne(s => s.PendingBlock)
            .HasForeignKey<PendingBlockEntity>(p=>p.PbftSequenceNumberId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.Status);
        builder.HasIndex(x => x.Hash).IsUnique();
        builder.HasIndex(x => x.PbftSequenceNumberId);
    }
}