using ElectronicVoting.Validator.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Configuration;

public class NodeConfiguration: IEntityTypeConfiguration<Node>
{
    public void Configure(EntityTypeBuilder<Node> builder)
    {
        builder.HasKey(x => x.Id);
        builder.Property(x=>x.Name).IsRequired();
        builder.Property(x=>x.Host).IsRequired();
        
        builder.HasIndex(a => a.Name).IsUnique();
        builder.HasIndex(a => a.Host).IsUnique();
        
        var conteainerName = Environment.GetEnvironmentVariable("CONTAINER_NAME");
        if(!string.IsNullOrEmpty(conteainerName))
        {
            Node[] nodes = new Node[2];
            if(conteainerName == "electronicvoting.validator.api.1")
            {
                nodes[0] = new Node();
                nodes[0].Id = 1;
                nodes[0].Host = "https://electronicvoting.validator.api.1:443";
                nodes[0].Name = "electronicvoting.validator.api.1";
                nodes[0].IsCurrent = true;

                nodes[1] = new Node();
                nodes[1].Id = 2;
                nodes[1].Host = "https://electronicvoting.validator.api.2:443";
                nodes[1].Name = "electronicvote.validator.api.2";
                nodes[1].IsCurrent = false;
            }
            else if(conteainerName == "electronicvoting.validator.api.2")
            {
                nodes[0] = new Node();
                nodes[0].Id=1;
                nodes[0].Host = "https://electronicvoting.validator.api.1:443";
                nodes[0].Name = "electronicvoting.validator.api.1";
                nodes[0].IsCurrent = false;

                nodes[1] = new Node();
                nodes[1].Id = 2;
                nodes[1].Host = "https://electronicvoting.validator.api.2:443";
                nodes[1].Name = "electronicvoting.validator.api.2";
                nodes[1].IsCurrent = true;
            }

            builder.HasData(nodes);
        }
    }
}