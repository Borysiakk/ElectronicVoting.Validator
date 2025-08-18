using ElectronicVoting.Validator.Domain.Entities.Election;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Configurations;

public class ElectionValidatorsConfiguration : IEntityTypeConfiguration<ValidatorNode>
{
    public void Configure(EntityTypeBuilder<ValidatorNode> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Name).IsRequired();
        builder.HasIndex(x => x.Name).IsUnique();
        builder.Property(x=>x.PublicKey).IsRequired(false);
        builder.Property(x=>x.ServerUrl).IsRequired();
        builder.Property(x => x.IsLeader).HasDefaultValue(false).IsRequired();
    }
}