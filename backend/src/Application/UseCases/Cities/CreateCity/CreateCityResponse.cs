namespace Application.UseCases.Cities.CreateCity;

public class CreateCityResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? MayorId { get; set; }
    public string? MayorName { get; set; }
    public DateTime CreatedAt { get; set; }
} 