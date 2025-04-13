using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using ElectronicVoting.Validator.Domain.Entities.Blockchain;

namespace ElectronicVoting.Validator.Infrastructure.Blockchain;

public static class HashCalculator
{
    public static string ComputeHash(PendingBlock block)
    {
        var blockData = PrepareBlockData(block.PbftSequenceNumber, block.PreviousHash, block.PendingTransactions?.Select(tx => tx.Id.ToString()));
        return ComputeHashFromJson(blockData);
    }

    public static string ComputeHash(Block block)
    {
        var blockData = PrepareBlockData(block.PbftSequenceNumber,block.PreviousHash, block.Transactions?.Select(tx => tx.Id.ToString()));
        return ComputeHashFromJson(blockData);
    }

    private static object PrepareBlockData(long sequenceNumber,string previousHash, IEnumerable<string>? transactions)
    {
        return new
        {
            SequenceNumber = sequenceNumber,
            PreviousHash = previousHash,
            Transactions = transactions
        };
    }

    private static string ComputeHashFromJson(object data)
    {
        var jsonData = JsonSerializer.Serialize(data);
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(jsonData));
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}