using ElectronicVoting.Validator.Domain.Entities;
using ElectronicVoting.Validator.Infrastructure.EntityFramework;
using Microsoft.EntityFrameworkCore;

namespace ElectronicVoting.Validator.Infrastructure.Repository;

public interface IApplicationSettingRepository: IRepository<ApplicationSetting>
{
    Task<string?> GetByKeyAsync(string key, CancellationToken cancellationToken = default);
    Task<T?> GetByKeyAsync<T>(string key, CancellationToken cancellationToken = default) where T : struct;
}

public class ApplicationSettingRepository: Repository<ApplicationSetting>, IApplicationSettingRepository
{
    public ApplicationSettingRepository(ValidatorDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<string?> GetByKeyAsync(string key, CancellationToken cancellationToken = default)
    {
        return (await DbSet.FirstOrDefaultAsync(a => a.Key == key, cancellationToken))?.Value;
    }

    public async Task<T?> GetByKeyAsync<T>(string key, CancellationToken cancellationToken = default) where T : struct
    {
        var value = await GetByKeyAsync(key, cancellationToken);
        
        if (string.IsNullOrWhiteSpace(value))
            return null;

        try
        {
            return (T?)Convert.ChangeType(value, typeof(T));
        }
        catch
        {
            return null;
        }
    }
}