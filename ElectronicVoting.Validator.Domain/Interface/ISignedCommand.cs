using System.Text.Json.Serialization;

namespace ElectronicVoting.Validator.Domain.Interface;

public interface ISignedCommand : ISignable
{
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    string Signature { get; set; }
    [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
    Guid SignedByValidatorId { get; set; }
}