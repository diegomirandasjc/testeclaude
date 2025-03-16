using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Infrastructure.Data;

public static class SeedData
{
    public static async Task Initialize(UserManager<User> userManager)
    {
        // Verifica se já existe algum usuário
        if (userManager.Users.Any())
            return;

        // Cria o usuário administrador
        var admin = new User
        {
            UserName = "admin@example.com",
            Email = "admin@example.com",
            FirstName = "Admin",
            LastName = "User",
            EmailConfirmed = true,
            CreatedAt = DateTime.UtcNow
        };

        // Adiciona o usuário com a senha
        await userManager.CreateAsync(admin, "Admin@123456");
    }
} 