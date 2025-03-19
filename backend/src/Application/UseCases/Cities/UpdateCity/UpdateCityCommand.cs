using MediatR;

namespace Application.UseCases.Cities.UpdateCity;

public class UpdateCityCommand : IRequest<UpdateCityResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid? MayorId { get; set; }
} 