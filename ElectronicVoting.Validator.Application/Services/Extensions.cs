using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ElectronicVoting.Validator.Application.Services.Api;
using ElectronicVoting.Validator.Domain.Interface.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ElectronicVoting.Validator.Infrastructure.Extensions;

using PemUtils;

namespace ElectronicVoting.Validator.Application.Services;

public static class Extensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection service, IConfiguration configuration)
    {
        service.AddScoped<ISignatureService>(_ => CreateSignatureService(configuration));
        service.AddScoped<IVoteValidationService, VoteValidationService>();
        service.AddScoped<IElectionValidatorService, ElectionValidatorService>();
        service.AddScoped<IValidationVoteApiService, ValidationVoteApiService>();
        service.AddScoped<IBlockValidationApiService, BlockValidationApiService>();
        service.AddScoped<IPendingBlockStorageService, PendingBlockStorageService>();
        return service;
    }

    private static ISignatureService CreateSignatureService(IConfiguration configuration)
    {
        var privateKeyString = configuration["Validator:PrivateKeyForVoteSigning"];
        if (string.IsNullOrEmpty(privateKeyString))
            throw new InvalidOperationException("Klucz prywatny nie został skonfigurowany");
            
        var normalizedPrivateKey = privateKeyString.Replace("\\n", "\n");
        var privateKey = RSA.Create();
        privateKey.ImportFromPem(normalizedPrivateKey);
            
        var caCertificateString = configuration["Validator:CaCertification"];
        if (string.IsNullOrEmpty(caCertificateString))
            throw new InvalidOperationException("Certyfikat CA nie został skonfigurowany");
            
        var caCertificate = X509Certificate2.CreateFromPem(caCertificateString);

        return new SignatureService(privateKey, caCertificate);
    }
}