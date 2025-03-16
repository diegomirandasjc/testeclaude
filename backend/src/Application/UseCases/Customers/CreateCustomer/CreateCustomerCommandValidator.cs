using FluentValidation;

namespace Application.UseCases.Customers.CreateCustomer;

public class CreateCustomerCommandValidator : AbstractValidator<CreateCustomerCommand>
{
    public CreateCustomerCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome do cliente é obrigatório")
            .MaximumLength(100).WithMessage("O nome do cliente não pode ter mais que 100 caracteres")
            .MinimumLength(3).WithMessage("O nome do cliente deve ter pelo menos 3 caracteres");

        RuleFor(x => x.Cpf)
            .NotEmpty().WithMessage("O CPF é obrigatório")
            .Length(11).WithMessage("O CPF deve ter 11 dígitos")
            .Matches("^[0-9]+$").WithMessage("O CPF deve conter apenas números");

        RuleFor(x => x.CityId)
            .NotEmpty().WithMessage("A cidade é obrigatória");
    }
} 