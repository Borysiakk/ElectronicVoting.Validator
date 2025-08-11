using System.Security.Cryptography;

namespace ElectronicVoting.Validator.Infrastructure.Extensions;

public static class CryptoExtensions
{
    public static ECDsa CreateECDsaFromBase64Key(string privateBas64Key)
    {
        try
        {
            var keyBytes = Convert.FromBase64String(privateBas64Key);
            var privateKeyEcDsa = ECDsa.Create();
            privateKeyEcDsa.ImportPkcs8PrivateKey(keyBytes, out _);
            return privateKeyEcDsa;
        }       
        catch (Exception)
        {
            throw new ArgumentException("Nieprawidłowy format klucza prywatnego");
        }
    }
}