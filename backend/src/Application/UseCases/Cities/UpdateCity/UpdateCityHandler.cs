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
        city.MayorId = request.MayorId;
        city.UpdatedAt = DateTime.UtcNow;

        var updatedCity = await _cityRepository.UpdateAsync(city, cancellationToken);

        return new UpdateCityResponse
        {
            Id = updatedCity.Id,
            Name = updatedCity.Name,
            MayorId = updatedCity.MayorId,
            MayorName = updatedCity.Mayor?.Name,
            CreatedAt = updatedCity.CreatedAt 
        };
    }
} 