using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface ICustomerRepository
{
    Task<PagedResult<Customer>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken);
    Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken);
    Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken);
    Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken);
    Task<Customer> GetByCpfAsync(string cpf, CancellationToken cancellationToken);
} 