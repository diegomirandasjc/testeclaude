namespace Domain.Entities;

public class Customer
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public Guid CityId { get; set; }
    public City City { get; set; }
    public DateTime CreatedAt { get; set; }
} 