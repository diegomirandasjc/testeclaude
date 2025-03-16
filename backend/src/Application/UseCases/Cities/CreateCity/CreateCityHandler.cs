using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using FluentValidation;

namespace Application.UseCases.Cities.CreateCity;

public class CreateCityHandler : IRequestHandler<CreateCityCommand, CreateCityResponse>
{
    private readonly ICityRepository _cityRepository;
    private readonly IValidator<CreateCityCommand> _validator;

    public CreateCityHandler(ICityRepository cityRepository, IValidator<CreateCityCommand> validator)
    {
        _cityRepository = cityRepository;
        _validator = validator;
    }

    public async Task<CreateCityResponse> Handle(CreateCityCommand request, CancellationToken cancellationToken)
    {
        // Validação dos campos
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verifica se já existe uma cidade com o mesmo nome
        var exists = await _cityRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (exists)
        {
            throw new ValidationException("Já existe uma cidade com este nome.");
        }

        var city = new City
        {
            Name = request.Name,
            CreatedAt = DateTime.UtcNow
        };

        var createdCity = await _cityRepository.CreateAsync(city, cancellationToken);

        return new CreateCityResponse
        {
            Id = createdCity.Id,
            Name = createdCity.Name,
            CreatedAt = createdCity.CreatedAt
        };
    }
} 