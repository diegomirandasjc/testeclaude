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
