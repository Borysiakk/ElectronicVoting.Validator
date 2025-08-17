using Amazon.S3;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ElectronicVoting.Validator.Infrastructure.Minio;

public static class Extensions
{
    public static IServiceCollection AddMinioS3(this IServiceCollection services, IConfiguration configuration)
    {
        var endpoint = configuration["MINIO_ENDPOINT"] ?? Environment.GetEnvironmentVariable("MINIO_ENDPOINT");
        var accessKey = configuration["MINIO_ACCESS_KEY"] ?? Environment.GetEnvironmentVariable("MINIO_ACCESS_KEY");
        var secretKey = configuration["MINIO_SECRET_KEY"] ?? Environment.GetEnvironmentVariable("MINIO_SECRET_KEY");
        
        if (string.IsNullOrEmpty(endpoint) || string.IsNullOrEmpty(accessKey) || string.IsNullOrEmpty(secretKey))
            throw new InvalidOperationException("Brak konfiguracji MinIO (MINIO_ENDPOINT / MINIO_ACCESS_KEY / MINIO_SECRET_KEY).");

        var s3Config = new AmazonS3Config
        {
            ServiceURL = $"http://{endpoint}",
            ForcePathStyle = true
        };

        var s3Client = new AmazonS3Client(accessKey, secretKey, s3Config);
        services.AddSingleton<IAmazonS3>(s3Client);

        services.AddSingleton<IMinioStorageService, MinioStorageService>();

        return services;
    }
}