namespace Domain.Entities;

public class City
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? MayorId { get; set; }
    public Person? Mayor { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
} 