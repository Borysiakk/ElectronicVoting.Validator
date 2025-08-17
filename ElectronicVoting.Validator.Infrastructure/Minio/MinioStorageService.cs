using Amazon.S3;
using Amazon.S3.Model;

namespace ElectronicVoting.Validator.Infrastructure.Minio;

public interface IMinioStorageService
{
    Task UploadAsync(string bucketName, string key, Stream data, CancellationToken ct = default);
    Task<Stream?> DownloadAsync(string bucketName, string key, CancellationToken ct = default);
}

public class MinioStorageService(IAmazonS3 s3Client) : IMinioStorageService
{
    public async Task UploadAsync(string bucketName, string key, Stream data, CancellationToken ct = default)
    {
        var putReq = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = data
        };
        await s3Client.PutObjectAsync(putReq, ct);
    }

    public async Task<Stream> DownloadAsync(string bucketName, string key, CancellationToken ct = default)
    {
        try
        {
            var resp = await s3Client.GetObjectAsync(new GetObjectRequest { BucketName = bucketName, Key = key }, ct);
            var ms = new MemoryStream();
            await resp.ResponseStream.CopyToAsync(ms, ct);
            ms.Position = 0;
            return ms;
        }
        catch (AmazonS3Exception e) when (e.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            return null;
        }

    }
}