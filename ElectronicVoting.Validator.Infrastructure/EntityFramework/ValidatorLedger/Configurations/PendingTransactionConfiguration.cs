using System.Text.Json;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Models;
using ElectronicVoting.Validator.Domain.Models.Election;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class PendingTransactionConfiguration: IEntityTypeConfiguration<PendingTransactionEntity>
{
    public void Configure(EntityTypeBuilder<PendingTransactionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        
        builder.HasOne(x => x.PendingBlock)
            .WithMany(b => b.PendingTransactions)
            .HasForeignKey(x => x.PendingBlockId)
            .OnDelete(DeleteBehavior.Restrict);
        
        builder.HasIndex(x => x.PendingBlockId);
        builder.HasIndex(x => x.VoteEncryptionId);
        builder.HasIndex(x => x.VoteValidationProcessId);
        
    }
}