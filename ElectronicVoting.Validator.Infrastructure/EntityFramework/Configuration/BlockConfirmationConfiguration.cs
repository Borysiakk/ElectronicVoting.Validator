using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;

public class BlockConfirmationConfiguration: IEntityTypeConfiguration<BlockConfirmation>
{
    public void Configure(EntityTypeBuilder<BlockConfirmation> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Type).IsRequired();
        builder.Property(x => x.PendingBlockId).IsRequired();
    }
}