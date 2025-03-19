#!/bin/bash
# Script para implementar o MetadataDependencyChecker no projeto
# Recomendado executar na raiz do projeto (onde está o .sln)

echo "Iniciando implementação do MetadataDependencyChecker..."

# Criar diretório para extensões se não existir
mkdir -p src/Infrastructure/Extensions

# 1. Criar interface no Domain
echo "Criando interface IMetadataDependencyChecker.cs..."
cat > src/Domain/Interfaces/IMetadataDependencyChecker.cs << 'EOL'
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Domain.Interfaces;

/// <summary>
/// Interface para o serviço de verificação de dependências baseado em metadados do EF Core
/// </summary>
public interface IMetadataDependencyChecker
{
    /// <summary>
    /// Identifica todas as entidades que possuem relacionamento com a entidade especificada
    /// </summary>
    /// <param name="entity">Entidade para verificar dependências</param>
    /// <returns>Lista de informações de dependência</returns>
    List<EntityRelationship> GetEntityRelationships(object entity);

    /// <summary>
    /// Identifica todas as entidades que possuem relacionamento com o tipo especificado
    /// </summary>
    /// <param name="entityType">Tipo da entidade</param>
    /// <returns>Lista de informações de dependência</returns>
    List<EntityRelationship> GetEntityRelationships(Type entityType);
}

/// <summary>
/// Representa um relacionamento entre entidades
/// </summary>
public class EntityRelationship
{
    /// <summary>
    /// Nome amigável da entidade
    /// </summary>
    public string EntityName { get; set; }
    
    /// <summary>
    /// Nome técnico do tipo da entidade
    /// </summary>
    public string EntityTypeName { get; set; }
    
    /// <summary>
    /// Nome da tabela no banco de dados
    /// </summary>
    public string TableName { get; set; }
    
    /// <summary>
    /// Lista de propriedades de chave estrangeira
    /// </summary>
    public List<string> ForeignKeyProperties { get; set; } = new List<string>();
    
    /// <summary>
    /// Lista de propriedades de navegação
    /// </summary>
    public List<string> NavigationProperties { get; set; } = new List<string>();
    
    /// <summary>
    /// Indica se o relacionamento é requerido
    /// </summary>
    public bool IsRequired { get; set; }
}
EOL

# 2. Criar implementação na Infrastructure
echo "Criando implementação MetadataDependencyChecker.cs..."
cat > src/Infrastructure/Services/MetadataDependencyChecker.cs << 'EOL'
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Infrastructure.Services;

/// <summary>
/// Serviço para verificação de dependências entre entidades usando metadados do EF Core
/// </summary>
public class MetadataDependencyChecker : IMetadataDependencyChecker
{
    private readonly DbContext _dbContext;
    private readonly Dictionary<string, string> _entityDisplayNames;

    public MetadataDependencyChecker(DbContext dbContext)
    {
        _dbContext = dbContext;
        
        _entityDisplayNames = new Dictionary<string, string>
        {
            {"Customer", "Clientes"},
            {"Product", "Produtos"},
            {"Order", "Pedidos"},
            {"Invoice", "Faturas"},
            {"Contract", "Contratos"},
            {"Employee", "Funcionários"},
            {"Supplier", "Fornecedores"},
            {"User", "Usuários"},
            {"Person", "Pessoas"},
            {"City", "Cidades"},
            // Adicione outros conforme necessário
        };
    }

    /// <inheritdoc />
    public List<EntityRelationship> GetEntityRelationships(object entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        
        var entityType = entity.GetType();
        return GetEntityRelationships(entityType);
    }

    /// <inheritdoc />
    public List<EntityRelationship> GetEntityRelationships(Type entityType)
    {
        var relationships = new List<EntityRelationship>();
        
        // Obtém o modelo de metadados do EF Core
        var model = _dbContext.Model;
        
        // Encontra o tipo da entidade no modelo
        var sourceEntityType = model.FindEntityType(entityType);
        if (sourceEntityType == null)
        {
            // Tenta encontrar por nome se não encontrar diretamente
            sourceEntityType = model.GetEntityTypes()
                .FirstOrDefault(e => e.ClrType.Name == entityType.Name);
                
            if (sourceEntityType == null)
                return relationships;
        }

        // Verifica todos os tipos de entidade no modelo
        foreach (var targetEntityType in model.GetEntityTypes())
        {
            // Pula a própria entidade
            if (targetEntityType == sourceEntityType) continue;

            // Verifica se o tipo alvo tem relacionamentos com o tipo fonte
            var entityRelationship = GetRelationshipInfo(sourceEntityType, targetEntityType);
            if (entityRelationship != null)
            {
                relationships.Add(entityRelationship);
            }
        }
        
        return relationships;
    }

    /// <summary>
    /// Obtém informações de relacionamento entre duas entidades
    /// </summary>
    private EntityRelationship GetRelationshipInfo(IEntityType sourceEntityType, IEntityType targetEntityType)
    {
        // Inicializa as listas de relações
        var foreignKeys = new List<string>();
        var navigations = new List<string>();
        
        // Verifica chaves estrangeiras que apontam para a fonte
        foreach (var fk in targetEntityType.GetForeignKeys())
        {
            if (fk.PrincipalEntityType == sourceEntityType)
            {
                var properties = string.Join(", ", fk.Properties.Select(p => p.Name));
                foreignKeys.Add($"{properties} -> {fk.PrincipalKey.Properties.First().Name}");
            }
        }
        
        // Verifica propriedades de navegação que apontam para a fonte
        foreach (var navigation in targetEntityType.GetNavigations())
        {
            if (navigation.TargetEntityType == sourceEntityType)
            {
                navigations.Add(navigation.Name);
            }
        }
        
        // Se não encontrou nenhuma relação, retorna null
        if (!foreignKeys.Any() && !navigations.Any())
            return null;
            
        // Cria o objeto de relacionamento
        return new EntityRelationship
        {
            EntityName = GetDisplayName(targetEntityType.Name),
            EntityTypeName = targetEntityType.Name,
            TableName = targetEntityType.GetTableName(),
            ForeignKeyProperties = foreignKeys,
            NavigationProperties = navigations,
            IsRequired = targetEntityType.GetForeignKeys()
                .Where(fk => fk.PrincipalEntityType == sourceEntityType)
                .Any(fk => !fk.IsRequired)
        };
    }

    /// <summary>
    /// Obtém um nome amigável para exibição
    /// </summary>
    private string GetDisplayName(string entityName)
    {
        if (_entityDisplayNames.TryGetValue(entityName, out var displayName))
            return displayName;
            
        // Formata o nome para melhor legibilidade
        var formattedName = Regex.Replace(
            entityName, "([a-z])([A-Z])", "$1 $2");
            
        // Tenta pluralizar para português
        if (formattedName.EndsWith("a"))
            return formattedName + "s";
        else if (formattedName.EndsWith("r"))
            return formattedName + "es";
        else
            return formattedName + "s";
    }
}
EOL

# 3. Criar as extensões
echo "Criando métodos de extensão DependencyExtensions.cs..."
cat > src/Infrastructure/Extensions/DependencyExtensions.cs << 'EOL'
using Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Infrastructure.Extensions
{
    /// <summary>
    /// Métodos de extensão para verificação de dependências
    /// </summary>
    public static class DependencyExtensions
    {
        /// <summary>
        /// Obtém os relacionamentos de uma entidade
        /// </summary>
        public static List<EntityRelationship> GetRelationships<TEntity>(
            this DbContext dbContext, 
            TEntity entity) 
            where TEntity : class
        {
            var checker = dbContext.GetService<IMetadataDependencyChecker>();
            return checker.GetEntityRelationships(entity);
        }

        /// <summary>
        /// Obtém os relacionamentos de um tipo de entidade
        /// </summary>
        public static List<EntityRelationship> GetRelationshipsForType<TEntity>(
            this DbContext dbContext) 
            where TEntity : class
        {
            var checker = dbContext.GetService<IMetadataDependencyChecker>();
            return checker.GetEntityRelationships(typeof(TEntity));
        }

        /// <summary>
        /// Verifica se um tipo de entidade tem relacionamentos com outro tipo
        /// </summary>
        public static bool HasRelationshipWith<TSource, TTarget>(this DbContext dbContext)
            where TSource : class
            where TTarget : class
        {
            var checker = dbContext.GetService<IMetadataDependencyChecker>();
            var relationships = checker.GetEntityRelationships(typeof(TSource));
            
            return relationships.Any(r => r.EntityTypeName == typeof(TTarget).Name);
        }
        
        /// <summary>
        /// Obtém um serviço do provedor de serviços interno do DbContext
        /// </summary>
        private static T GetService<T>(this DbContext context) where T : class
        {
            return (T)context.GetService(typeof(T));
        }
        
        /// <summary>
        /// Obtém um serviço do DbContext
        /// </summary>
        private static object GetService(this DbContext context, Type type)
        {
            var dbContextType = context.GetType();
            var serviceProvider = dbContextType.GetProperty("InternalServiceProvider")?.GetValue(context);
            
            if (serviceProvider == null)
                throw new InvalidOperationException("Não foi possível obter o provedor de serviços interno do DbContext");
                
            var getServiceMethod = serviceProvider.GetType().GetMethod("GetService") 
                ?? throw new InvalidOperationException("Método GetService não encontrado");
                
            return getServiceMethod.Invoke(serviceProvider, new[] { type });
        }
    }
}
EOL

# 4. Atualizar o DependencyInjection.cs
echo "Atualizando DependencyInjection.cs para registrar o serviço..."
# Backup do arquivo original
cp src/Infrastructure/DependencyInjection.cs src/Infrastructure/DependencyInjection.cs.bak

cat > src/Infrastructure/DependencyInjection.cs << 'EOL'
using Domain.Interfaces;
using Infrastructure.Data;
using Infrastructure.Repositories;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        // Repositórios
        services.AddScoped<ICityRepository, CityRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IPersonRepository, PersonRepository>();
        
        // Serviços
        services.AddScoped<IMetadataDependencyChecker, MetadataDependencyChecker>(provider => 
        {
            // Obtém a instância do DbContext
            var dbContext = provider.GetRequiredService<ApplicationDbContext>();
            return new MetadataDependencyChecker(dbContext);
        });
        
        return services;
    }
}
EOL

# 5. Atualizar o CheckPersonDependenciesHandler
echo "Atualizando CheckPersonDependenciesHandler.cs..."
# Backup do arquivo original
cp src/Application/UseCases/Persons/CheckPersonDependencies/CheckPersonDependenciesHandler.cs src/Application/UseCases/Persons/CheckPersonDependencies/CheckPersonDependenciesHandler.cs.bak

cat > src/Application/UseCases/Persons/CheckPersonDependencies/CheckPersonDependenciesHandler.cs << 'EOL'
using Domain.Interfaces;
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Persons.CheckPersonDependencies;

public class CheckPersonDependenciesHandler : IRequestHandler<CheckPersonDependenciesQuery, CheckPersonDependenciesResponse>
{
    private readonly IPersonRepository _personRepository;
    private readonly IMetadataDependencyChecker _dependencyChecker;

    public CheckPersonDependenciesHandler(
        IPersonRepository personRepository, 
        IMetadataDependencyChecker dependencyChecker)
    {
        _personRepository = personRepository;
        _dependencyChecker = dependencyChecker;
    }

    public async Task<CheckPersonDependenciesResponse> Handle(
        CheckPersonDependenciesQuery request, 
        CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.Id, cancellationToken);
        if (person == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        // Usa o serviço para verificar dependências
        var relationships = _dependencyChecker.GetEntityRelationships(person);

        return new CheckPersonDependenciesResponse
        {
            HasDependencies = relationships.Any(),
            Dependencies = relationships.Select(r => r.EntityName).Distinct().ToList()
        };
    }
}
EOL

# 6. Atualizar o CheckPersonDependenciesResponse se necessário
cp src/Application/UseCases/Persons/CheckPersonDependencies/CheckPersonDependenciesQuery.cs src/Application/UseCases/Persons/CheckPersonDependencies/CheckPersonDependenciesQuery.cs.bak

cat > src/Application/UseCases/Persons/CheckPersonDependencies/CheckPersonDependenciesQuery.cs << 'EOL'
using Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;

namespace Application.UseCases.Persons.CheckPersonDependencies;

public class CheckPersonDependenciesQuery : IRequest<CheckPersonDependenciesResponse>
{
    public Guid Id { get; set; }
}

public class CheckPersonDependenciesResponse
{
    public bool HasDependencies { get; set; }
    public List<string> Dependencies { get; set; } = new();
    public List<EntityRelationship> DetailedDependencies { get; set; } = new();
}
EOL

# 7. Criar o Handler genérico (opcional)
echo "Criando handler genérico para verificação de dependências..."
mkdir -p src/Application/UseCases/Common/CheckEntityDependencies

cat > src/Application/UseCases/Common/CheckEntityDependencies/CheckEntityDependenciesQuery.cs << 'EOL'
using Domain.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.UseCases.Common.CheckEntityDependencies;

// Query para verificar dependências de qualquer entidade
public class CheckEntityDependenciesQuery : IRequest<EntityDependenciesResponse>
{
    public Guid Id { get; set; }
    public string EntityType { get; set; }
}

// Resposta com informações de dependência
public class EntityDependenciesResponse
{
    public bool HasDependencies { get; set; }
    public List<string> RelatedEntities { get; set; } = new List<string>();
    public List<EntityRelationship> DetailedRelationships { get; set; } = new List<EntityRelationship>();
}

// Handler genérico para verificar dependências de qualquer tipo de entidade
public class CheckEntityDependenciesHandler : IRequestHandler<CheckEntityDependenciesQuery, EntityDependenciesResponse>
{
    private readonly DbContext _dbContext;
    private readonly IMetadataDependencyChecker _dependencyChecker;
    private readonly Dictionary<string, Type> _entityTypes;

    public CheckEntityDependenciesHandler(
        DbContext dbContext,
        IMetadataDependencyChecker dependencyChecker)
    {
        _dbContext = dbContext;
        _dependencyChecker = dependencyChecker;
        
        // Inicializa o mapeamento de nomes para tipos de entidade
        _entityTypes = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.Namespace?.StartsWith("Domain.Entities") == true)
            .ToDictionary(t => t.Name, t => t, StringComparer.OrdinalIgnoreCase);
    }

    public async Task<EntityDependenciesResponse> Handle(
        CheckEntityDependenciesQuery request, 
        CancellationToken cancellationToken)
    {
        // Verifica se o tipo de entidade existe
        if (!_entityTypes.TryGetValue(request.EntityType, out var entityType))
        {
            throw new ArgumentException($"Tipo de entidade '{request.EntityType}' não encontrado.");
        }

        // Tenta localizar a entidade no banco de dados
        var entity = await _dbContext.FindAsync(entityType, request.Id);
        if (entity == null)
        {
            throw new KeyNotFoundException($"Entidade do tipo '{request.EntityType}' com ID '{request.Id}' não encontrada.");
        }

        // Obtém os relacionamentos usando o verificador de metadados
        var relationships = _dependencyChecker.GetEntityRelationships(entity);

        // Retorna a resposta
        return new EntityDependenciesResponse
        {
            HasDependencies = relationships.Any(),
            RelatedEntities = relationships.Select(r => r.EntityName).ToList(),
            DetailedRelationships = relationships
        };
    }
}
EOL

# 8. Criar um controller para verificação genérica (opcional)
echo "Criando controller para verificação genérica de dependências..."
cat > src/Api/Controllers/DependenciesController.cs << 'EOL'
using Application.UseCases.Common.CheckEntityDependencies;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Api.Controllers;

[ApiController]
[Route("api/dependencies")]
[Authorize]
public class DependenciesController : ControllerBase
{
    private readonly IMediator _mediator;

    public DependenciesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("{entityType}/{id}")]
    public async Task<ActionResult<EntityDependenciesResponse>> CheckDependencies(string entityType, Guid id)
    {
        try
        {
            var response = await _mediator.Send(new CheckEntityDependenciesQuery 
            { 
                EntityType = entityType,
                Id = id
            });
            
            return Ok(response);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
EOL

echo "Implementação concluída com sucesso!"
echo "Para verificar se tudo está funcionando corretamente, compile o projeto:"
echo "dotnet build"
