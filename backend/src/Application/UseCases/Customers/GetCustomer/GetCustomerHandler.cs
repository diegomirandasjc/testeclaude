using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Customers.GetCustomer;

public class GetCustomerHandler : IRequestHandler<GetCustomerQuery, GetCustomerResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public GetCustomerHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<GetCustomerResponse> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.Id, cancellationToken);
        if (customer == null)
        {
            throw new KeyNotFoundException("Cliente não encontrado.");
        }

        return new GetCustomerResponse
        {
            Id = customer.Id,
            Name = customer.Name,
            Cpf = customer.Cpf,
            CityId = customer.CityId,
            CityName = customer.City.Name,
            CreatedAt = customer.CreatedAt
        };
    }
} 