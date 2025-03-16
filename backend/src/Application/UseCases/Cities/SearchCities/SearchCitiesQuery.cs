using MediatR;

namespace Application.UseCases.Cities.SearchCities;

public class SearchCitiesQuery : IRequest<SearchCitiesResponse>
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
} 