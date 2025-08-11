using System.Security.Cryptography.X509Certificates;

namespace ElectronicVoting.Validator.Domain.Interface.Services;

public interface ISignatureService
{
    string Sign(ISignable vote);
    bool Verify(ISignable vote, X509Certificate2 publicKey, string base64Signature);
}