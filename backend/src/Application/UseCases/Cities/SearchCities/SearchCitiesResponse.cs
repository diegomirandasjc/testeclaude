namespace Application.UseCases.Cities.SearchCities;

public class SearchCitiesResponse
{
    public List<CityDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class CityDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? MayorId { get; set; }
    public string? MayorName { get; set; }
    public DateTime CreatedAt { get; set; }
} 