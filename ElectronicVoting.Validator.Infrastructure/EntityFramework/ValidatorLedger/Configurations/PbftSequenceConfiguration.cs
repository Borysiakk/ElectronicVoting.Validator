using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class PbftSequenceConfiguration: IEntityTypeConfiguration<PbftSequenceEntity>
{
    public void Configure(EntityTypeBuilder<PbftSequenceEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.Property(a => a.SequenceNumber).IsRequired();
        builder.HasIndex(a => a.SequenceNumber);
        
        builder.HasOne(x => x.Block)
            .WithOne(pb => pb.PbftSequence)
            .HasForeignKey<BlockEntity>(pb => pb.PbftSequenceNumberId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}