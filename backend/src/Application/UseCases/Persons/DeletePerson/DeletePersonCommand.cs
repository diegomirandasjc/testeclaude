using MediatR;

namespace Application.UseCases.Persons.DeletePerson;

public class DeletePersonCommand : IRequest<DeletePersonResponse>
{
    public Guid Id { get; set; }
}

public class DeletePersonResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
} 