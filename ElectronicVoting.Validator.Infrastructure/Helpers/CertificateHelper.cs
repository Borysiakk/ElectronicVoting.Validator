using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace ElectronicVoting.Validator.Infrastructure.Helpers;

public static class CertificateHelper
{
    public static X509Certificate2 GetCertificateFromString(string certificateText)
    {
        var certificateString = certificateText.Replace("\\n", "\n");
        var certificate = RSA.Create();
        return X509Certificate2.CreateFromPem(certificateString);
    }

}
