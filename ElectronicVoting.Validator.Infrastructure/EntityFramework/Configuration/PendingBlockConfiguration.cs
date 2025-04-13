using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;

public class PendingBlockConfiguration: IEntityTypeConfiguration<PendingBlock>
{
    public void Configure(EntityTypeBuilder<PendingBlock> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Id).ValueGeneratedNever(); 
        builder.Property(x => x.PreviousHash).IsRequired(true);
        builder.Property(x => x.IsProcessed).IsRequired(true).HasDefaultValue(false);

        builder
            .HasMany(e => e.PendingTransactions)
            .WithOne(ev => ev.PendingBlock)
            .HasForeignKey(ev => ev.PendingBlockId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(false);
    }
}