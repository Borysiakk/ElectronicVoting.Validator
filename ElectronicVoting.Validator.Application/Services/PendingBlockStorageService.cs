using System.Text;
using System.Text.Json;
using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Infrastructure.Minio;
using FluentResults;

namespace ElectronicVoting.Validator.Application.Services;

public interface IPendingBlockStorageService
{
    Task<Result> StorePendingBlockAsync(PendingBlockDto block, CancellationToken cancellationToken);
    Task<Result<PendingBlockDto>> GetPendingBlockAsync(string blockId, CancellationToken cancellationToken);
}

public class PendingBlockStorageService(IMinioStorageService minioStorageService) : IPendingBlockStorageService
{
    private const string BucketName = "pending-blocks";
    
    public async Task<Result> StorePendingBlockAsync(PendingBlockDto block, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(block, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            using var stream = new MemoryStream(jsonBytes);
            
            var key = $"pending-block-{block.Id}.json";
            await minioStorageService.UploadAsync(BucketName, key, stream, cancellationToken);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to store pending block: {ex.Message}");
        }

    }

    public async Task<Result<PendingBlockDto>> GetPendingBlockAsync(string blockId, CancellationToken cancellationToken)
    {
        try
        {
            var key = $"pending-block-{blockId}.json";
            var stream = await minioStorageService.DownloadAsync(BucketName, key, cancellationToken);
            
            if (stream == null)
                return Result.Ok<PendingBlockDto?>(null);
            
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync(cancellationToken);
            
            var block = JsonSerializer.Deserialize<PendingBlockDto>(json, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            return Result.Ok<PendingBlockDto>(block);
        }
        catch (Exception ex)
        {
            return Result.Fail<PendingBlockDto>($"Failed to get pending block: {ex.Message}");
        }

    }
}