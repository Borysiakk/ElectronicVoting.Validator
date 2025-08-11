using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using ElectronicVoting.Validator.Domain.Interface;

namespace ElectronicVoting.Validator.Application.Services;

public interface IVoteSignatureService
{
    string Sign(ISignable vote);
    bool Verify(ISignable vote, X509Certificate2 publicKey, string base64Signature);
}

public class VoteSignatureServiceb(RSA privateKey, X509Certificate2 caCertificate) : IVoteSignatureService
{
    private readonly RSA _privateKey = privateKey;
    private readonly X509Certificate2 _caCertificate = caCertificate;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    
    public string Sign(ISignable vote)
    {
        var data = Serialize(vote);
        var signatureBytes = _privateKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signatureBytes);
    }

    public bool Verify(ISignable vote, X509Certificate2 publicKey, string base64Signature)
    {
        if (!VerifyCertificateWithCa(publicKey))
            throw new UnauthorizedAccessException("Certyfikat nie jest zaufany - nie został podpisany przez CA");
        
        var data = Serialize(vote);
        var signatureBytes = Convert.FromBase64String(base64Signature);
        using var rsa = publicKey.GetRSAPublicKey();
        if (rsa == null)
            throw new InvalidOperationException("Certyfikat nie zawiera klucza RSA lub klucz jest nieprawidłowy");
        
        return rsa.VerifyData(data, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
    
    private static byte[] Serialize(object obj)
    {
        var json = JsonSerializer.Serialize(obj, _jsonOptions);
        return Encoding.UTF8.GetBytes(json);
    }
     
    private bool VerifyCertificateWithCa(X509Certificate2 certificate)
    {
        var chain = new X509Chain();
        chain.ChainPolicy.ExtraStore.Add(_caCertificate);
        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllowUnknownCertificateAuthority;
        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;

        return chain.Build(certificate);
    }

}