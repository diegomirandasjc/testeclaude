using MediatR;

namespace Application.UseCases.Persons.CheckPersonDependencies;

public class CheckPersonDependenciesQuery : IRequest<CheckPersonDependenciesResponse>
{
    public Guid Id { get; set; }
}

public class CheckPersonDependenciesResponse
{
    public bool HasDependencies { get; set; }
    public List<string> Dependencies { get; set; } = new();
} 