using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface ICityRepository
{
    Task<PagedResult<City>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken);
    Task<City> CreateAsync(City city, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<City> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<City> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<City> UpdateAsync(City city, CancellationToken cancellationToken);
} 