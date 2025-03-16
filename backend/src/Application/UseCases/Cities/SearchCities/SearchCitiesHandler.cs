using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Cities.SearchCities;

public class SearchCitiesHandler : IRequestHandler<SearchCitiesQuery, SearchCitiesResponse>
{
    private readonly ICityRepository _cityRepository;

    public SearchCitiesHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<SearchCitiesResponse> Handle(SearchCitiesQuery request, CancellationToken cancellationToken)
    {
        var result = await _cityRepository.SearchAsync(
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new SearchCitiesResponse
        {
            Items = result.Items.Select(c => new CityDto
            {
                Id = c.Id,
                Name = c.Name,
                CreatedAt = c.CreatedAt
            }).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
} 