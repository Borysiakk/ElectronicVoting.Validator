using System.Text;
using System.Text.Json;
using ElectronicVoting.Validator.Application.DTO.BlockValidation;
using ElectronicVoting.Validator.Infrastructure.Minio;
using FluentResults;

namespace ElectronicVoting.Validator.Application.Services;

public interface IPendingBlockStorageService
{
    Task<Result> StoreGetPendingBlockDetailsAsync(PendingBlockDetailsDto block, CancellationToken cancellationToken);
    Task<Result<PendingBlockDetailsDto>> GetPendingBlockDetailsAsync(Guid blockId, CancellationToken cancellationToken);
}

public class PendingBlockStorageService(IMinioStorageService minioStorageService) : IPendingBlockStorageService
{
    private const string BucketName = "pending-blocks";
    
    public async Task<Result> StoreGetPendingBlockDetailsAsync(PendingBlockDetailsDto pendingBlockDetails, CancellationToken cancellationToken)
    {
        try
        {
            var json = JsonSerializer.Serialize(pendingBlockDetails, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            var jsonBytes = Encoding.UTF8.GetBytes(json);
            using var stream = new MemoryStream(jsonBytes);
            
            var key = $"pending-block-{pendingBlockDetails.Id}.json";
            await minioStorageService.UploadAsync(BucketName, key, stream, cancellationToken);
            
            return Result.Ok();
        }
        catch (Exception ex)
        {
            return Result.Fail($"Failed to store pending block: {ex.Message}");
        }
    }

    public async Task<Result<PendingBlockDetailsDto>> GetPendingBlockDetailsAsync(Guid blockId, CancellationToken cancellationToken)
    {
        try
        {
            var key = $"pending-block-{blockId}.json";
            var stream = await minioStorageService.DownloadAsync(BucketName, key, cancellationToken);
            
            if (stream == null)
                return Result.Ok<PendingBlockDetailsDto>(null);
            
            using var reader = new StreamReader(stream);
            var json = await reader.ReadToEndAsync(cancellationToken);
            
            var block = JsonSerializer.Deserialize<PendingBlockDetailsDto>(json, new JsonSerializerOptions 
            { 
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase 
            });
            
            return Result.Ok<PendingBlockDetailsDto>(block);
        }
        catch (Exception ex)
        {
            return Result.Fail<PendingBlockDetailsDto>($"Failed to get pending block: {ex.Message}");
        }

    }
}