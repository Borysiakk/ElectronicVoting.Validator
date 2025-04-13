namespace ElectronicVoting.Validator.Domain.Entities;

public class Node : Entity
{
    public string Name { get; set; }
    public string Host { get; set; }
    public bool IsCurrent { get; set; }
}