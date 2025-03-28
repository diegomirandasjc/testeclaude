using MediatR;

namespace Application.UseCases.Customers.UpdateCustomer;

public class UpdateCustomerCommand : IRequest<UpdateCustomerResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public Guid CityId { get; set; }
}

public class UpdateCustomerResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Cpf { get; set; }
    public Guid CityId { get; set; }
    public string CityName { get; set; }
    public DateTime CreatedAt { get; set; }
} 