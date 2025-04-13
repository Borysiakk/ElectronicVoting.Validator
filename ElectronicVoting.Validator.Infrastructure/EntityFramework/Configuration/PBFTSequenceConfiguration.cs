using ElectronicVoting.Validator.Domain.Entities.Blockchain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;

public class PBFTSequenceConfiguration : IEntityTypeConfiguration<PBFTSequence>
{
    public void Configure(EntityTypeBuilder<PBFTSequence> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.SequenceNumber).IsRequired().ValueGeneratedNever();
    }
}