using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.UseCases.Users.GenerateToken;

public class GenerateTokenHandler : IRequestHandler<GenerateTokenCommand, string>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenService _tokenService;

    public GenerateTokenHandler(UserManager<User> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    public async Task<string> Handle(GenerateTokenCommand request, CancellationToken cancellationToken)
    {
        var user = await _userManager.FindByIdAsync(request.UserId);
        if (user == null)
            throw new Exception("User not found");

        return _tokenService.GenerateToken(user);
    }
} 