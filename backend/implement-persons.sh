#!/bin/bash
# Script para implementação do cadastro de Pessoas

echo "Iniciando implementação do cadastro de Pessoas..."

# Criar diretórios para a entidade Person
echo "Criando estrutura de pastas..."
mkdir -p src/Domain/Entities
mkdir -p src/Domain/Interfaces
mkdir -p src/Application/UseCases/Persons/CreatePerson
mkdir -p src/Application/UseCases/Persons/GetPerson
mkdir -p src/Application/UseCases/Persons/SearchPersons
mkdir -p src/Application/UseCases/Persons/UpdatePerson
mkdir -p src/Api/Controllers
mkdir -p src/Infrastructure/Repositories

# Domain/Entities/Person.cs
echo "Criando Domain/Entities/Person.cs..."
cat > src/Domain/Entities/Person.cs << 'EOL'
namespace Domain.Entities;

public class Person
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
EOL

# Domain/Interfaces/IPersonRepository.cs
echo "Criando Domain/Interfaces/IPersonRepository.cs..."
cat > src/Domain/Interfaces/IPersonRepository.cs << 'EOL'
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
}
EOL

# Application/UseCases/Persons/CreatePerson/CreatePersonCommand.cs
echo "Criando CreatePersonCommand.cs..."
cat > src/Application/UseCases/Persons/CreatePerson/CreatePersonCommand.cs << 'EOL'
using MediatR;

namespace Application.UseCases.Persons.CreatePerson;

public class CreatePersonCommand : IRequest<CreatePersonResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
EOL

# Application/UseCases/Persons/CreatePerson/CreatePersonCommandValidator.cs
echo "Criando CreatePersonCommandValidator.cs..."
cat > src/Application/UseCases/Persons/CreatePerson/CreatePersonCommandValidator.cs << 'EOL'
using FluentValidation;

namespace Application.UseCases.Persons.CreatePerson;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da pessoa é obrigatório")
            .MaximumLength(100).WithMessage("O nome da pessoa não pode ter mais que 100 caracteres")
            .MinimumLength(3).WithMessage("O nome da pessoa deve ter pelo menos 3 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email informado não é válido")
            .MaximumLength(100).WithMessage("O email não pode ter mais que 100 caracteres");
    }
}
EOL

# Application/UseCases/Persons/CreatePerson/CreatePersonHandler.cs
echo "Criando CreatePersonHandler.cs..."
cat > src/Application/UseCases/Persons/CreatePerson/CreatePersonHandler.cs << 'EOL'
using Domain.Entities;
using Domain.Interfaces;
using MediatR;
using FluentValidation;

namespace Application.UseCases.Persons.CreatePerson;

public class CreatePersonHandler : IRequestHandler<CreatePersonCommand, CreatePersonResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly IValidator<CreatePersonCommand> _validator;

    public CreatePersonHandler(IPersonRepository personRepository, IValidator<CreatePersonCommand> validator)
    {
        _personRepository = personRepository;
        _validator = validator;
    }

    public async Task<CreatePersonResponse> Handle(CreatePersonCommand request, CancellationToken cancellationToken)
    {
        // Validação dos campos
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        // Verifica se já existe uma pessoa com o mesmo email
        var exists = await _personRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (exists)
        {
            throw new ValidationException("Já existe uma pessoa cadastrada com este email.");
        }

        var person = new Person
        {
            Name = request.Name,
            Email = request.Email,
            CreatedAt = DateTime.UtcNow
        };

        var createdPerson = await _personRepository.CreateAsync(person, cancellationToken);

        return new CreatePersonResponse
        {
            Id = createdPerson.Id,
            Name = createdPerson.Name,
            Email = createdPerson.Email,
            CreatedAt = createdPerson.CreatedAt
        };
    }
}
EOL

# Application/UseCases/Persons/CreatePerson/CreatePersonResponse.cs
echo "Criando CreatePersonResponse.cs..."
cat > src/Application/UseCases/Persons/CreatePerson/CreatePersonResponse.cs << 'EOL'
namespace Application.UseCases.Persons.CreatePerson;

public class CreatePersonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
EOL

# Application/UseCases/Persons/GetPerson/GetPersonQuery.cs
echo "Criando GetPersonQuery.cs..."
cat > src/Application/UseCases/Persons/GetPerson/GetPersonQuery.cs << 'EOL'
using MediatR;

namespace Application.UseCases.Persons.GetPerson;

public class GetPersonQuery : IRequest<GetPersonResponse>
{
    public Guid Id { get; set; }
}

public class GetPersonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
EOL

# Application/UseCases/Persons/GetPerson/GetPersonHandler.cs
echo "Criando GetPersonHandler.cs..."
cat > src/Application/UseCases/Persons/GetPerson/GetPersonHandler.cs << 'EOL'
using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Persons.GetPerson;

public class GetPersonHandler : IRequestHandler<GetPersonQuery, GetPersonResponse>
{
    private readonly IPersonRepository _personRepository;

    public GetPersonHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<GetPersonResponse> Handle(GetPersonQuery request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.Id, cancellationToken);
        if (person == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        return new GetPersonResponse
        {
            Id = person.Id,
            Name = person.Name,
            Email = person.Email,
            CreatedAt = person.CreatedAt
        };
    }
}
EOL

# Application/UseCases/Persons/SearchPersons/SearchPersonsQuery.cs
echo "Criando SearchPersonsQuery.cs..."
cat > src/Application/UseCases/Persons/SearchPersons/SearchPersonsQuery.cs << 'EOL'
using MediatR;

namespace Application.UseCases.Persons.SearchPersons;

public class SearchPersonsQuery : IRequest<SearchPersonsResponse>
{
    public string? SearchTerm { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
EOL

# Application/UseCases/Persons/SearchPersons/SearchPersonsResponse.cs
echo "Criando SearchPersonsResponse.cs..."
cat > src/Application/UseCases/Persons/SearchPersons/SearchPersonsResponse.cs << 'EOL'
namespace Application.UseCases.Persons.SearchPersons;

public class SearchPersonsResponse
{
    public List<PersonDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

public class PersonDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
EOL

# Application/UseCases/Persons/SearchPersons/SearchPersonsHandler.cs
echo "Criando SearchPersonsHandler.cs..."
cat > src/Application/UseCases/Persons/SearchPersons/SearchPersonsHandler.cs << 'EOL'
using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Persons.SearchPersons;

public class SearchPersonsHandler : IRequestHandler<SearchPersonsQuery, SearchPersonsResponse>
{
    private readonly IPersonRepository _personRepository;

    public SearchPersonsHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<SearchPersonsResponse> Handle(SearchPersonsQuery request, CancellationToken cancellationToken)
    {
        var result = await _personRepository.SearchAsync(
            request.SearchTerm,
            request.Page,
            request.PageSize,
            cancellationToken);

        return new SearchPersonsResponse
        {
            Items = result.Items.Select(p => new PersonDto
            {
                Id = p.Id,
                Name = p.Name,
                Email = p.Email,
                CreatedAt = p.CreatedAt
            }).ToList(),
            TotalCount = result.TotalCount,
            Page = result.Page,
            PageSize = result.PageSize,
            TotalPages = result.TotalPages
        };
    }
}
EOL

# Application/UseCases/Persons/UpdatePerson/UpdatePersonCommand.cs
echo "Criando UpdatePersonCommand.cs..."
cat > src/Application/UseCases/Persons/UpdatePerson/UpdatePersonCommand.cs << 'EOL'
using MediatR;

namespace Application.UseCases.Persons.UpdatePerson;

public class UpdatePersonCommand : IRequest<UpdatePersonResponse>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UpdatePersonResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
EOL

# Application/UseCases/Persons/UpdatePerson/UpdatePersonValidator.cs
echo "Criando UpdatePersonValidator.cs..."
cat > src/Application/UseCases/Persons/UpdatePerson/UpdatePersonValidator.cs << 'EOL'
using FluentValidation;

namespace Application.UseCases.Persons.UpdatePerson;

public class UpdatePersonValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email informado não é válido")
            .MaximumLength(100).WithMessage("O email não pode ter mais de 100 caracteres");
    }
}
EOL

# Application/UseCases/Persons/UpdatePerson/UpdatePersonHandler.cs
echo "Criando UpdatePersonHandler.cs..."
cat > src/Application/UseCases/Persons/UpdatePerson/UpdatePersonHandler.cs << 'EOL'
using Domain.Interfaces;
using MediatR;
using FluentValidation;

namespace Application.UseCases.Persons.UpdatePerson;

public class UpdatePersonHandler : IRequestHandler<UpdatePersonCommand, UpdatePersonResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly IValidator<UpdatePersonCommand> _validator;

    public UpdatePersonHandler(IPersonRepository personRepository, IValidator<UpdatePersonCommand> validator)
    {
        _personRepository = personRepository;
        _validator = validator;
    }

    public async Task<UpdatePersonResponse> Handle(UpdatePersonCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            throw new ValidationException(validationResult.Errors);
        }

        var person = await _personRepository.GetByIdAsync(request.Id, cancellationToken);
        if (person == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        // Verifica se já existe outra pessoa com o mesmo email
        var existingPerson = await _personRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingPerson != null && existingPerson.Id != request.Id)
        {
            throw new ValidationException("Já existe uma pessoa cadastrada com este email.");
        }

        person.Name = request.Name;
        person.Email = request.Email;
        person.UpdatedAt = DateTime.UtcNow;

        await _personRepository.UpdateAsync(person, cancellationToken);

        return new UpdatePersonResponse
        {
            Id = person.Id,
            Name = person.Name,
            Email = person.Email,
            CreatedAt = person.CreatedAt,
            UpdatedAt = person.UpdatedAt
        };
    }
}
EOL

# Api/Controllers/PersonsController.cs
echo "Criando PersonsController.cs..."
cat > src/Api/Controllers/PersonsController.cs << 'EOL'
using Application.UseCases.Persons.CreatePerson;
using Application.UseCases.Persons.GetPerson;
using Application.UseCases.Persons.SearchPersons;
using Application.UseCases.Persons.UpdatePerson;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PersonsController : ControllerBase
{
    private readonly IMediator _mediator;

    public PersonsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<SearchPersonsResponse>> Search([FromQuery] SearchPersonsQuery query)
    {
        var response = await _mediator.Send(query);
        return Ok(response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<GetPersonResponse>> Get(Guid id)
    {
        try
        {
            var response = await _mediator.Send(new GetPersonQuery { Id = id });
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<ActionResult<CreatePersonResponse>> Create(CreatePersonCommand command)
    {
        try
        {
            var response = await _mediator.Send(command);
            return CreatedAtAction(nameof(Get), new { id = response.Id }, response);
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(errors);
        }
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UpdatePersonResponse>> Update(Guid id, UpdatePersonCommand command)
    {
        try
        {
            command.Id = id;
            var response = await _mediator.Send(command);
            return Ok(response);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
        catch (ValidationException ex)
        {
            var errors = ex.Errors
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(x => x.ErrorMessage).ToArray()
                );

            return BadRequest(errors);
        }
    }
}
EOL

# Infrastructure/Repositories/PersonRepository.cs
echo "Criando PersonRepository.cs..."
cat > src/Infrastructure/Repositories/PersonRepository.cs << 'EOL'
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
}
EOL

# Backup dos arquivos originais
echo "Fazendo backup dos arquivos originais..."
cp src/Domain/Interfaces/IApplicationDbContext.cs src/Domain/Interfaces/IApplicationDbContext.cs.bak
cp src/Infrastructure/Data/ApplicationDbContext.cs src/Infrastructure/Data/ApplicationDbContext.cs.bak
cp src/Infrastructure/DependencyInjection.cs src/Infrastructure/DependencyInjection.cs.bak

# Modificação 1: Adicionar Persons ao IApplicationDbContext
echo "Modificando IApplicationDbContext.cs para adicionar Persons..."
cat > src/Domain/Interfaces/IApplicationDbContext.cs << 'EOL'
using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Domain.Interfaces;

public interface IApplicationDbContext
{
    DbSet<City> Cities { get; set; }
    DbSet<Customer> Customers { get; set; }
    DbSet<Product> Products { get; set; }
    DbSet<Place> Places { get; set; }
    DbSet<Person> Persons { get; set; } // Nova propriedade para Persons
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
EOL

# Modificação 2: Adicionar novo trecho ao método OnModelCreating do ApplicationDbContext
echo "ATENÇÃO: Instruções para modificar ApplicationDbContext.cs:"
echo "Abra o arquivo src/Infrastructure/Data/ApplicationDbContext.cs"
echo "No método OnModelCreating, adicione o seguinte código antes do fim do método (antes do último }):"
echo ""
echo "builder.Entity<Person>(entity =>"
echo "{"
echo "    entity.HasKey(e => e.Id);"
echo "    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);"
echo "    entity.Property(e => e.Email).IsRequired().HasMaxLength(100);"
echo "    entity.Property(e => e.CreatedAt).IsRequired();"
echo "});"
echo ""
echo "builder.Entity<Person>()"
echo "    .HasIndex(p => p.Email)"
echo "    .IsUnique();"
echo ""

# Modificação 3: Adicionar registro do repositório no DependencyInjection
echo "Modificando DependencyInjection.cs para registrar PersonRepository..."
cat > src/Infrastructure/DependencyInjection.cs << 'EOL'
using Domain.Interfaces;
using Infrastructure.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPlaceRepository, PlaceRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>(); // Adicionado
        return services;
    }
}
EOL

echo "Criação dos arquivos concluída com sucesso!"
echo ""
echo "Para finalizar a implementação:"
echo "Modifique manualmente o arquivo src/Infrastructure/Data/ApplicationDbContext.cs conforme as instruções acima"
echo "Execute os comandos para criar e aplicar a migration:"
echo "dotnet ef migrations add AddPersonsTable --project src/Infrastructure --startup-project src/Api"
echo "dotnet ef database update --project src/Infrastructure --startup-project src/Api"
echo ""
echo "Após isso, o cadastro de Pessoas estará disponível na API!"
