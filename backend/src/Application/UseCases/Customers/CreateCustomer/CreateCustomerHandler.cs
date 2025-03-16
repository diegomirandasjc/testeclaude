using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Customers.CreateCustomer;

public class CreateCustomerHandler : IRequestHandler<CreateCustomerCommand, CreateCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IValidator<CreateCustomerCommand> _validator;

    public CreateCustomerHandler(
        ICustomerRepository customerRepository,
        ICityRepository cityRepository,
        IValidator<CreateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _cityRepository = cityRepository;
        _validator = validator;
    }

    public async Task<CreateCustomerResponse> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        // Validação dos campos
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verifica se já existe um cliente com o mesmo CPF
        var exists = await _customerRepository.ExistsByCpfAsync(request.Cpf, cancellationToken);
        if (exists)
        {
            throw new ValidationException("Já existe um cliente com este CPF.");
        }

        // Verifica se a cidade existe
        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city == null)
        {
            throw new ValidationException("Cidade não encontrada.");
        }

        var customer = new Customer
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Cpf = request.Cpf,
            CityId = request.CityId,
            CreatedAt = DateTime.UtcNow
        };

        var createdCustomer = await _customerRepository.CreateAsync(customer, cancellationToken);

        return new CreateCustomerResponse
        {
            Id = createdCustomer.Id,
            Name = createdCustomer.Name,
            Cpf = createdCustomer.Cpf,
            CityId = createdCustomer.CityId,
            CityName = city.Name,
            CreatedAt = createdCustomer.CreatedAt
        };
    }
} 