using ElectronicVoting.Validator.Domain.Entities.ValidatorLedger;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.ValidatorLedger.Configurations;

public class BlockValidationResultConfiguration : IEntityTypeConfiguration<BlockValidationResultEntity>
{
    public void Configure(EntityTypeBuilder<BlockValidationResultEntity> builder)
    {
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.PendingBlockEntity)
            .WithMany() // PendingBlockEntity może mieć wiele BlockValidationResults
            .HasForeignKey(x => x.PendingBlockEntityId)
            .OnDelete(DeleteBehavior.Cascade); // lub DeleteBehavior.Restrict w zależności od potrzeb
        

        builder.Property(x => x.Hash)
            .IsRequired()
            .HasMaxLength(256); 
        
        builder.Property(x => x.ValidatorId)
            .IsRequired();
        
        builder.Property(x => x.IsValid)
            .IsRequired();
        
        builder.Property(x => x.IsLeaderValidation)
            .IsRequired();
        
        builder.Property(x => x.RejectionReason)
            .HasConversion(
                v => v != null ? string.Join(';', v) : null, // konwersja do string
                v => !string.IsNullOrEmpty(v) ? v.Split(';', StringSplitOptions.RemoveEmptyEntries).ToList() : new List<string>()) // konwersja z string
            .HasColumnName("RejectionReasons");


    }
}