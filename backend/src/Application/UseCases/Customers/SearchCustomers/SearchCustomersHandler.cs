using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Customers.SearchCustomers;

public class SearchCustomersHandler : IRequestHandler<SearchCustomersQuery, SearchCustomersResponse>
{
    private readonly ICustomerRepository _customerRepository;

    public SearchCustomersHandler(ICustomerRepository customerRepository)
    {
        _customerRepository = customerRepository;
    }

    public async Task<SearchCustomersResponse> Handle(SearchCustomersQuery request, CancellationToken cancellationToken)
    {
        var result = await _customerRepository.SearchAsync(
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new SearchCustomersResponse
        {
            Items = result.Items.Select(c => new CustomerDto
            {
                Id = c.Id,
                Name = c.Name,
                Cpf = c.Cpf,
                CityId = c.CityId,
                CityName = c.City.Name,
                CreatedAt = c.CreatedAt
            }).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
} 