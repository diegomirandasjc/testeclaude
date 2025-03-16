using MediatR;

namespace Application.UseCases.Cities.GetCity;

public class GetCityQuery : IRequest<GetCityResponse>
{
    public Guid Id { get; set; }
}

public class GetCityResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
} 