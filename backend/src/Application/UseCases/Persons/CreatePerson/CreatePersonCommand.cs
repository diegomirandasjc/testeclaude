using MediatR;

namespace Application.UseCases.Persons.CreatePerson;

public class CreatePersonCommand : IRequest<CreatePersonResponse>
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
