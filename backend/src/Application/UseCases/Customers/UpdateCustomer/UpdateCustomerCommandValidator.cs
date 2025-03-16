using FluentValidation;

namespace Application.UseCases.Customers.UpdateCustomer;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
    public UpdateCustomerCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("O ID do cliente é obrigatório.");

        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("O nome do cliente é obrigatório.")
            .MaximumLength(100)
            .WithMessage("O nome do cliente não pode ter mais que 100 caracteres.");

        RuleFor(x => x.Cpf)
            .NotEmpty()
            .WithMessage("O CPF do cliente é obrigatório.")
            .Length(11)
            .WithMessage("O CPF deve ter 11 dígitos.");

        RuleFor(x => x.CityId)
            .NotEmpty()
            .WithMessage("A cidade do cliente é obrigatória.");
    }
} 