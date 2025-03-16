using MediatR;

namespace Application.UseCases.Customers.CreateCustomer;

public class CreateCustomerCommand : IRequest<CreateCustomerResponse>
{
    public string Name { get; set; }
    public string Cpf { get; set; }
    public Guid CityId { get; set; }
}

public class CreateCustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public Guid CityId { get; set; }
    public string CityName { get; set; }
    public DateTime CreatedAt { get; set; }
} 