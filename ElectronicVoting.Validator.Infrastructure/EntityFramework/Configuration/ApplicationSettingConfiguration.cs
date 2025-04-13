using ElectronicVoting.Validator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;

public class ApplicationSettingConfiguration: IEntityTypeConfiguration<ApplicationSetting>
{
    public void Configure(EntityTypeBuilder<ApplicationSetting> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x => x.Key).IsRequired();
        builder.Property(x => x.Value).IsRequired();
        
        builder.HasData(new ApplicationSetting[]
        {
            new ApplicationSetting
            {
                Id = 1,
                Key = "TransactionThresholdForBlock",
                Value = "1"
            }
        });
    }
}