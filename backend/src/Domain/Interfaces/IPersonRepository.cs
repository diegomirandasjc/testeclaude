using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IPersonRepository
{
    Task<PagedResult<Person>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken);
    Task<Person> CreateAsync(Person person, CancellationToken cancellationToken);
    Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Person> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Person> GetByEmailAsync(string email, CancellationToken cancellationToken);
    Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}
