using MediatR;

namespace Application.UseCases.Cities.CreateCity;

public class CreateCityCommand : IRequest<CreateCityResponse>
{
    public string Name { get; set; } = string.Empty;
} 