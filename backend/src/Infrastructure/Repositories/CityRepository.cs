using Domain.Common;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class CityRepository : ICityRepository
{
    private readonly IApplicationDbContext _context;

    public CityRepository(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PagedResult<City>> SearchAsync(string searchTerm, int page, int pageSize, CancellationToken cancellationToken)
    {
        var query = _context.Cities
            .Include(c => c.Mayor)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            query = query.Where(c => c.Name.ToLower().Contains(searchTerm.ToLower()));
        }

        query = query.OrderBy(c => c.Name);

        return await query.ToPagedResultAsync(page, pageSize, cancellationToken);
    }

    public async Task<City> CreateAsync(City city, CancellationToken cancellationToken)
    {
        _context.Cities.Add(city);
        await _context.SaveChangesAsync(cancellationToken);
        return city;
    }

    public async Task<bool> ExistsByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Cities
            .AnyAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<City> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _context.Cities
            .Include(c => c.Mayor)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }

    public async Task<City> GetByNameAsync(string name, CancellationToken cancellationToken)
    {
        return await _context.Cities
            .Include(c => c.Mayor)
            .FirstOrDefaultAsync(c => c.Name.ToLower() == name.ToLower(), cancellationToken);
    }

    public async Task<City> UpdateAsync(City city, CancellationToken cancellationToken)
    {
        _context.Cities.Update(city);
        await _context.SaveChangesAsync(cancellationToken);
        
        // Recarrega a cidade com o relacionamento do prefeito
        return await _context.Cities
            .Include(c => c.Mayor)
            .FirstOrDefaultAsync(c => c.Id == city.Id, cancellationToken);
    }
} 