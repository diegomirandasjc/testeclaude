using Domain.Interfaces;
using MediatR;

namespace Application.UseCases.Persons.DeletePerson;

public class DeletePersonHandler : IRequestHandler<DeletePersonCommand, DeletePersonResponse>
{
    private readonly IPersonRepository _personRepository;

    public DeletePersonHandler(IPersonRepository personRepository)
    {
        _personRepository = personRepository;
    }

    public async Task<DeletePersonResponse> Handle(DeletePersonCommand request, CancellationToken cancellationToken)
    {
        var person = await _personRepository.GetByIdAsync(request.Id, cancellationToken);
        if (person == null)
        {
            throw new KeyNotFoundException("Pessoa não encontrada.");
        }

        await _personRepository.DeleteAsync(request.Id, cancellationToken);

        return new DeletePersonResponse
        {
            Success = true,
            Message = "Pessoa excluída com sucesso."
        };
    }
} 