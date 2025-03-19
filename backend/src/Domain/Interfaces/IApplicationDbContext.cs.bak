using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<City> Cities { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Product> Products { get; set; }
 
    DbSet<Person> Persons { get; set; } // Nova propriedade para Persons
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
