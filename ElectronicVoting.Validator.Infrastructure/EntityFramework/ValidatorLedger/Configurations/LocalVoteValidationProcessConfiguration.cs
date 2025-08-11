using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class LocalVoteValidationProcessConfiguration : IEntityTypeConfiguration<LocalVoteValidationProcessEntity>
{
    public void Configure(EntityTypeBuilder<LocalVoteValidationProcessEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.HasIndex(a => a.VoteEncryptionId);
        builder.HasIndex(a => a.VoteValidationProcessId);
    }
}