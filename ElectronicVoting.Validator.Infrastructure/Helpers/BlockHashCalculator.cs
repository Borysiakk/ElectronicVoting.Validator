using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ElectronicVoting.Validator.Domain.Interface;
using ElectronicVoting.Validator.Domain.Interface.Services;

namespace ElectronicVoting.Validator.Infrastructure.Helpers;

public class BlockHash  
{
    public static string ComputeBlockHash(IHashable hashable)
    {
        var serialized = JsonSerializer.Serialize(hashable.GetHashSource());
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(serialized));
        return Convert.ToHexString(hashBytes);
    }

    public static bool HasValidHash(string originalHash, string reconstructedHash)
    {
        return string.Equals(originalHash, reconstructedHash, StringComparison.Ordinal);
    }
}
