namespace ElectronicVoting.Validator.Domain.Entities;

public class ApplicationSetting: Entity
{
    public string Key { get; set; }
    public string? Value { get; set; }
}