using FluentValidation;

namespace Application.UseCases.Cities.CreateCity;

public class CreateCityCommandValidator : AbstractValidator<CreateCityCommand>
{
    public CreateCityCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da cidade é obrigatório")
            .MaximumLength(100).WithMessage("O nome da cidade não pode ter mais que 100 caracteres")
            .MinimumLength(3).WithMessage("O nome da cidade deve ter pelo menos 3 caracteres");
    }
} 