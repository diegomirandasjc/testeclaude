using Domain.Interfaces;
using MediatR;
using FluentValidation;

namespace Application.UseCases.Cities.UpdateCity;

public class UpdateCityHandler : IRequestHandler<UpdateCityCommand, UpdateCityResponse>
{
    private readonly ICityRepository _cityRepository;
    private readonly IValidator<UpdateCityCommand> _validator;

    public UpdateCityHandler(ICityRepository cityRepository, IValidator<UpdateCityCommand> validator)
    {
        _cityRepository = cityRepository;
        _validator = validator;
    }

    public async Task<UpdateCityResponse> Handle(UpdateCityCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var city = await _cityRepository.GetByIdAsync(request.Id, cancellationToken);
        if (city == null)
        {
            throw new KeyNotFoundException("Cidade não encontrada.");
        }

        // Verifica se já existe outra cidade com o mesmo nome
        var existingCity = await _cityRepository.GetByNameAsync(request.Name, cancellationToken);
        if (existingCity != null && existingCity.Id != request.Id)
        {
            throw new ValidationException("Já existe uma cidade com este nome.");
        }

        city.Name = request.Name;
        city.UpdatedAt = DateTime.UtcNow;

        await _cityRepository.UpdateAsync(city, cancellationToken);

        return new UpdateCityResponse
        {
            Id = city.Id,
            Name = city.Name,
            CreatedAt = city.CreatedAt,
            UpdatedAt = city.UpdatedAt
        };
    }
} 