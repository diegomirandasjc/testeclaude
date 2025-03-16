using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CustomerRepository : ICustomerRepository
{
    private readonly IApplicationDbContext _context;

    public CustomerRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Customer>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Customers
            .Include(c => c.City)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            searchTerm = searchTerm.ToLower();
            query = query.Where(c => 
                c.Name.ToLower().Contains(searchTerm) || 
                c.Cpf.Contains(searchTerm));
        }

        query = query.OrderBy(c => c.Name);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<Customer> CreateAsync(Customer customer, CancellationToken cancellationToken)
    {
        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<bool> ExistsByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        return await _context.Customers
            .AnyAsync(c => c.Cpf == cpf, cancellationToken);
    }

    public async Task<Customer> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Customers
            .Include(c => c.City)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<Customer> UpdateAsync(Customer customer, CancellationToken cancellationToken)
    {
        _context.Customers.Update(customer);
        await _context.SaveChangesAsync(cancellationToken);
        return customer;
    }

    public async Task<Customer> GetByCpfAsync(string cpf, CancellationToken cancellationToken)
    {
        return await _context.Customers
            .Include(c => c.City)
            .FirstOrDefaultAsync(c => c.Cpf == cpf, cancellationToken);
    }
} 