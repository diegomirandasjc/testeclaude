using Domain.Entities;
using Domain.Interfaces;
using FluentValidation;
using MediatR;

namespace Application.UseCases.Customers.UpdateCustomer;

public class UpdateCustomerHandler : IRequestHandler<UpdateCustomerCommand, UpdateCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IValidator<UpdateCustomerCommand> _validator;

    public UpdateCustomerHandler(
        ICustomerRepository customerRepository,
        ICityRepository cityRepository,
        IValidator<UpdateCustomerCommand> validator)
    {
        _customerRepository = customerRepository;
        _cityRepository = cityRepository;
        _validator = validator;
    }

    public async Task<UpdateCustomerResponse> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            throw new KeyNotFoundException("Cliente não encontrado.");
        }

        var city = await _cityRepository.GetByIdAsync(request.CityId, cancellationToken);
        if (city == null)
        {
            throw new KeyNotFoundException("Cidade não encontrada.");
        }

        var existingCustomerWithCpf = await _customerRepository.GetByCpfAsync(request.Cpf, cancellationToken);
        if (existingCustomerWithCpf != null && existingCustomerWithCpf.Id != request.Id)
        {
            throw new ValidationException("Já existe um cliente cadastrado com este CPF.");
        }

        customer.Name = request.Name;
        customer.Cpf = request.Cpf;
        customer.CityId = request.CityId;

        var updatedCustomer = await _customerRepository.UpdateAsync(customer, cancellationToken);

        return new UpdateCustomerResponse
        {
            Id = updatedCustomer.Id,
            Name = updatedCustomer.Name,
            Cpf = updatedCustomer.Cpf,
            CityId = updatedCustomer.CityId,
            CityName = updatedCustomer.City.Name,
            CreatedAt = updatedCustomer.CreatedAt
        };
    }
} 