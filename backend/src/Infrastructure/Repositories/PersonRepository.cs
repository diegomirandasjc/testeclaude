using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class PersonRepository : IPersonRepository
{
    private readonly IApplicationDbContext _context;

    public PersonRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<Person>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Persons.AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(p => p.Name.ToLower().Contains(searchTerm.ToLower()) || 
                                     p.Email.ToLower().Contains(searchTerm.ToLower()));
        }

        query = query.OrderBy(p => p.Name);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<Person> CreateAsync(Person person, CancellationToken cancellationToken)
    {
        _context.Persons.Add(person);
        await _context.SaveChangesAsync(cancellationToken);
        return person;
    }

    public async Task<bool> ExistsByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Persons
            .AnyAsync(p => p.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<Person> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Persons.FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task<Person> GetByEmailAsync(string email, CancellationToken cancellationToken)
    {
        return await _context.Persons
            .FirstOrDefaultAsync(p => p.Email.ToLower() == email.ToLower(), cancellationToken);
    }

    public async Task<Person> UpdateAsync(Person person, CancellationToken cancellationToken)
    {
        _context.Persons.Update(person);
        await _context.SaveChangesAsync(cancellationToken);
        return person;
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        var person = await GetByIdAsync(id, cancellationToken);
        if (person != null)
        {
            _context.Persons.Remove(person);
            await _context.SaveChangesAsync(cancellationToken);
        }
    }
}
