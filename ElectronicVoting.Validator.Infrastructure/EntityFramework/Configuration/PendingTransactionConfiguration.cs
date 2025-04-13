using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;

public class PendingTransactionConfiguration: IEntityTypeConfiguration<PendingTransaction>
{
    public void Configure(EntityTypeBuilder<PendingTransaction> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever(); 
        builder.Property(x=>x.IsProcessed).HasDefaultValue(false);

        builder
            .HasOne(ev => ev.PendingBlock)
            .WithMany(e => e.PendingTransactions)
            .HasForeignKey(ev => ev.PendingBlockId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}