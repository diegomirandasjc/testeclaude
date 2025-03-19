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
