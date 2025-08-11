using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class VoteValidationResultConfiguration: IEntityTypeConfiguration<VoteValidationResultEntity>
{
    public void Configure(EntityTypeBuilder<VoteValidationResultEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.ValidatorId).IsRequired();
        builder.Property(a => a.VoteEncryptionId).IsRequired();
        builder.Property(a => a.VoteValidationProcessId).IsRequired();
        builder.Property(a => a.LocalVoteValidationProcessId).IsRequired();
        
        builder.HasIndex(a => a.ValidatorId);
        builder.HasIndex(a => a.VoteEncryptionId);
        builder.HasIndex(a => a.VoteValidationProcessId);
        builder.HasIndex(a => a.LocalVoteValidationProcessId);
    }
}