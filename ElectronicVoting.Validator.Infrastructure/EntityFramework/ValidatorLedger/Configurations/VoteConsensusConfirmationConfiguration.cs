using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class VoteConsensusConfirmationConfiguration: IEntityTypeConfiguration<VoteConsensusConfirmationEntity>
{
    public void Configure(EntityTypeBuilder<VoteConsensusConfirmationEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.VoteEncryptionId).IsRequired();
        builder.Property(a => a.VoteValidationProcessId).IsRequired();
        builder.HasIndex(a=>a.VoteEncryptionId);
        builder.HasIndex(a=>a.VoteValidationProcessId);
    }
}