using System.Text.Json;
using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using ElectronicVoting.Validator.Domain.Models.Election;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class VoteEncryptionConfiguration: IEntityTypeConfiguration<VoteEncryptionEntity>
{
    public void Configure(EntityTypeBuilder<VoteEncryptionEntity> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(a => a.Id).ValueGeneratedNever();
        builder.Property(x => x.VoteEncryption)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null),
                v => JsonSerializer.Deserialize<VoteEncryption>(v, (JsonSerializerOptions)null)
            )
            .HasColumnType("jsonb");
    }
}