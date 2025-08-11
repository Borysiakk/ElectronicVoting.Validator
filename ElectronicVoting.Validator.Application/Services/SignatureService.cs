using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Interface.Services;

namespace ElectronicVoting.Validator.Application.Services;
public class SignatureService(RSA privateKey, X509Certificate2 caCertificate) : ISignatureService
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false,
        DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
    };
    
    public string Sign(ISignable vote)
    {
        var data = SerializeWithoutSignature(vote);
        var signatureBytes = privateKey.SignData(data, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        return Convert.ToBase64String(signatureBytes);
    }

    public bool Verify(ISignable vote, X509Certificate2 publicKey, string base64Signature)
    {
        if (!VerifyCertificateWithCa(publicKey))
            throw new UnauthorizedAccessException("Certyfikat nie jest zaufany - nie został podpisany przez CA");
        
        var data = SerializeWithoutSignature(vote);
        var signatureBytes = Convert.FromBase64String(base64Signature);
        
        using var rsa = publicKey.GetRSAPublicKey();
        
        if (rsa == null)
            throw new InvalidOperationException("Certyfikat nie zawiera klucza RSA lub klucz jest nieprawidłowy");
        
        return rsa.VerifyData(data, signatureBytes, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
    }
    
    private static byte[] SerializeWithoutSignature(object obj)
    {
        var json = JsonSerializer.Serialize(obj, _jsonOptions);
        using var doc = JsonDocument.Parse(json);
        using var ms = new MemoryStream();
        using var writer = new Utf8JsonWriter(ms);

        writer.WriteStartObject();
        foreach (var prop in doc.RootElement.EnumerateObject())
        {
            if (prop.NameEquals("signature")) continue;
            prop.WriteTo(writer);
        }
        writer.WriteEndObject();
        writer.Flush();

        return ms.ToArray();
    }
    
    private bool VerifyCertificateWithCa(X509Certificate2 certificate)
    {
        using var chain = new X509Chain();

        chain.ChainPolicy.VerificationFlags = X509VerificationFlags.NoFlag;

        chain.ChainPolicy.RevocationMode = X509RevocationMode.NoCheck;
        chain.ChainPolicy.RevocationFlag = X509RevocationFlag.ExcludeRoot;

        chain.ChainPolicy.TrustMode = X509ChainTrustMode.CustomRootTrust;
        chain.ChainPolicy.CustomTrustStore.Clear();
        chain.ChainPolicy.CustomTrustStore.Add(caCertificate);
        
        if (!chain.Build(certificate))
            return false;
        
        var rootElement = chain.ChainElements[chain.ChainElements.Count - 1].Certificate;
        return string.Equals(rootElement.Thumbprint, caCertificate.Thumbprint, StringComparison.OrdinalIgnoreCase);

    }
}