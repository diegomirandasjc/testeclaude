using Domain.Common;
using Domain.Entities;

namespace Domain.Interfaces;

public interface IProductRepository
{
    Task<PagedResult<Product>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken);
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken);
    Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken);
    Task<Product> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<Product> GetByNameAsync(string name, CancellationToken cancellationToken);
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken);
} 