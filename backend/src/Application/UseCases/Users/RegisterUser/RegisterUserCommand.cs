using MediatR;

namespace Application.UseCases.Users.RegisterUser;

public class RegisterUserCommand : IRequest<RegisterUserResponse>
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
} 