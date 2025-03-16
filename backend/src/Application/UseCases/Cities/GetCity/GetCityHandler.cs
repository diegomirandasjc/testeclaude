using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Cities.GetCity;

public class GetCityHandler : IRequestHandler<GetCityQuery, GetCityResponse>
{
    private readonly ICityRepository _cityRepository;

    public GetCityHandler(ICityRepository cityRepository)
    {
        _cityRepository = cityRepository;
    }

    public async Task<GetCityResponse> Handle(GetCityQuery request, CancellationToken cancellationToken)
    {
        var city = await _cityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (city == null)
        {
            throw new KeyNotFoundException("Cidade não encontrada.");
        }

        return new GetCityResponse
        {
            Id = city.Id,
            Name = city.Name,
            CreatedAt = city.CreatedAt
        };
    }
} 