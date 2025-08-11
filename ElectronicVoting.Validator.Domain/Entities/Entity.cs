namespace ElectronicVoting.Validator.Domain.Entities;

public class Entity<TId>
{
    public TId Id { get; set; }
    public DateTime? CreatedDate { get; set; }
    public DateTime? ModifiedDate { get; set; }
}