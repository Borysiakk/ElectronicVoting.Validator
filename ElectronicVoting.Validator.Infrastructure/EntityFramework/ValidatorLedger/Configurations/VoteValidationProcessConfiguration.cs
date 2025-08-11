using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class VoteValidationProcessConfiguration: IEntityTypeConfiguration<VoteValidationProcessEntity>
{
    public void Configure(EntityTypeBuilder<VoteValidationProcessEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        
        builder.Property(a=>a.VoteEncryptionId).IsRequired();
        builder.HasIndex(a=>a.VoteEncryptionId);
        
        builder.Property(a => a.Status).HasConversion<string>().HasMaxLength(32).IsRequired();
        builder.HasIndex(a => a.Status);
    }
}