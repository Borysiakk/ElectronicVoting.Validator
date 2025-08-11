using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using ElectronicVoting.Validator.Application.Services;
using ElectronicVoting.Validator.Test.CertificateTools;
using Xunit;

namespace ElectronicVoting.Validator.Test.UnitTests.Sign;

public class SignatureServiceTests
{
    [Fact]
    public void VerifySignature_WithCertificateSignedByTrustedCa_ReturnsTrue()
    {
        var caCertification = CertificateGenerator.GenerateRootCaPemPair("CN=Ca, O=MyOrg, C=PL");
        var certification = CertificateGenerator.GenerateCrtPemPair("CN=apiA, O=MyOrg, C=PL", caCertification.PrivateKeyRaw, caCertification.PublicKey);;
        
        using var rsaPrivate = RSA.Create();
        rsaPrivate.ImportFromPem(certification.PrivateKey.AsSpan());
        
        var cert = X509Certificate2.CreateFromPem(certification.PublicKey);
        
        var caCert = X509Certificate2.CreateFromPem(caCertification.PublicKey);
        using var rsaPub = caCert.GetRSAPublicKey()!;
        
        SignatureService signatureService = new (rsaPrivate, caCert);
        TestSign sign = new TestSign()
        {
            TestFiled = Guid.NewGuid().ToString(),
        };

        var signature = signatureService.Sign(sign);
        var result = signatureService.Verify(sign, cert, signature);
        
        Assert.True(result);
    }
}