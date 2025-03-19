using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.UseCases.Persons.CheckPersonDependencies;

public class CheckPersonDependenciesHandler : IRequestHandler<CheckPersonDependenciesQuery, CheckPersonDependenciesResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly IApplicationDbContext _context;

    public CheckPersonDependenciesHandler(IPersonRepository personRepository, IApplicationDbContext context)
    {
        _personRepository = personRepository;
        _context = context;
    }

    public async Task<CheckPersonDependenciesResponse> Handle(CheckPersonDependenciesQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.Id, cancellationToken);
        if (person == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        var dependencies = new List<string>();

        // Verifica se a pessoa está sendo usada em clientes
        var hasCustomers = await _context.Customers
            .AnyAsync(c => c.Id == request.Id, cancellationToken);
        if (hasCustomers)
        {
            dependencies.Add("Clientes");
        }

        // Verifica se a pessoa está sendo usada em produtos
        var hasProducts = await _context.Products
            .AnyAsync(p => p.Id == request.Id, cancellationToken);
        if (hasProducts)
        {
            dependencies.Add("Produtos");
        }

        return new CheckPersonDependenciesResponse
        {
            HasDependencies = dependencies.Any(),
            Dependencies = dependencies
        };
    }
} 