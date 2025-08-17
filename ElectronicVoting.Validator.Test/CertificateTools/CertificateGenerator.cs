using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ElectronicVoting.Validator.Test.CertificateTools;

public class CertificateGenerator

{
    private static readonly int KeySize = 2048;
    private static readonly int KeySizeForCA = 2048;
    
    public static CertificateKey GenerateCrtPemPair(string subjectName, string caPrivateKeyPem, string caPublicKeyPem)
    {
        var caKeyDer = Convert.FromBase64String(caPrivateKeyPem);
        using var issuerRsa = RSA.Create();
        issuerRsa.ImportPkcs8PrivateKey(caKeyDer, out _);

        var caCert = X509Certificate2.CreateFromPem(caPublicKeyPem);
        var caCertWithKey = caCert.CopyWithPrivateKey(issuerRsa);
        
        using var rsa = RSA.Create(KeySize);
        var req = new CertificateRequest(new X500DistinguishedName(subjectName), rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, true));
        
        var serialNumber = new byte[16];
        RandomNumberGenerator.Fill(serialNumber);
        
        var notBefore = DateTimeOffset.UtcNow.AddMinutes(-5);
        var notAfter  = notBefore.AddYears(1);
        using var signedCert = req.Create(caCertWithKey, notBefore, notAfter, serialNumber);
        
        var derCert = signedCert.Export(X509ContentType.Cert); // DER
        var certB64 = Convert.ToBase64String(derCert, Base64FormattingOptions.InsertLineBreaks);
        string certPem = $"-----BEGIN CERTIFICATE-----\n{certB64}\n-----END CERTIFICATE-----\n";
        
        var pkcs8 = rsa.ExportPkcs8PrivateKey();
        var prvB64 = Convert.ToBase64String(pkcs8, Base64FormattingOptions.InsertLineBreaks);
        string privatePem = $"-----BEGIN PRIVATE KEY-----\n{prvB64}\n-----END PRIVATE KEY-----\n";
        
        return new CertificateKey
        {
            PrivateKey = privatePem,
            PrivateKeyRaw = prvB64,
            PublicKey = certPem,
            PublicKeyRaw = certB64
        };
    }

    public static CertificateKey GenerateRootCaPemPair(string subjectCommonName)
    {
        using var rsa = RSA.Create(KeySizeForCA);

        var req = new CertificateRequest(subjectCommonName, rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
        req.CertificateExtensions.Add(new X509BasicConstraintsExtension(true, false, 0, true));
        req.CertificateExtensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyCertSign | X509KeyUsageFlags.CrlSign, true));
        req.CertificateExtensions.Add(new X509SubjectKeyIdentifierExtension(req.PublicKey, false));

        var notBefore = DateTimeOffset.UtcNow.AddMinutes(-5);
        var notAfter = notBefore.AddYears(10);
        using var cert = req.CreateSelfSigned(notBefore, notAfter);
        
        var derCert = cert.Export(X509ContentType.Cert); // DER
        var certB64 = Convert.ToBase64String(derCert, Base64FormattingOptions.InsertLineBreaks);
        string certPem = $"-----BEGIN CERTIFICATE-----\n{certB64}\n-----END CERTIFICATE-----\n";
        
        var pkcs8 = rsa.ExportPkcs8PrivateKey();
        var prvB64 = Convert.ToBase64String(pkcs8, Base64FormattingOptions.InsertLineBreaks);
        string privatePem = $"-----BEGIN PRIVATE KEY-----\n{prvB64}\n-----END PRIVATE KEY-----\n";
        
        return new CertificateKey()
        {
            PrivateKey = privatePem,
            PrivateKeyRaw = prvB64,
            PublicKey = certPem,
            PublicKeyRaw = certB64
        };
    }
    
    private static string DerToPem(byte[] der, string label)
    {
        var b64 = Convert.ToBase64String(der, Base64FormattingOptions.InsertLineBreaks);
        return $"-----BEGIN {label}-----\n{b64}\n-----END {label}-----\n";
    }

}  