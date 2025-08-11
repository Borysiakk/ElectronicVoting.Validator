using ElectronicVoting.Validator.Domain.Entities.Election;
using ElectronicVoting.Validator.Infrastructure.EntityFramework.Election.Configurations;
using ElectronicVoting.Validator.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;


namespace ElectronicVoting.Validator.Infrastructure.EntityFramework.Election;

public class ElectionDbContext(DbContextOptions<ElectionDbContext> options, IHostEnvironment env) : DbContext(options)
{
    
    public DbSet<ValidatorNode> ValidatorNodes { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new ElectionValidatorsConfiguration(env?.IsDevelopmentDocker() ?? false));
    }
}