using MediatR;

namespace Application.UseCases.Customers.GetCustomer;

public class GetCustomerQuery : IRequest<GetCustomerResponse>
{
    public Guid Id { get; set; }
}

public class GetCustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public Guid CityId { get; set; }
    public string CityName { get; set; }
    public DateTime CreatedAt { get; set; }
} 