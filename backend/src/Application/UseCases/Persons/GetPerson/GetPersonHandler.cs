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
