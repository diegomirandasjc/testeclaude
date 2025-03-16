using MediatR;

namespace Application.UseCases.Users.GenerateToken;

public class GenerateTokenCommand : IRequest<string>
{
    public string UserId { get; set; }
} 