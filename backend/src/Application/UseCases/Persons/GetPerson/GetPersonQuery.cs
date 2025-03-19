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
