using FluentValidation;

namespace Application.UseCases.Persons.CreatePerson;

public class CreatePersonCommandValidator : AbstractValidator<CreatePersonCommand>
{
    public CreatePersonCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome da pessoa é obrigatório")
            .MaximumLength(100).WithMessage("O nome da pessoa não pode ter mais que 100 caracteres")
            .MinimumLength(3).WithMessage("O nome da pessoa deve ter pelo menos 3 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email informado não é válido")
            .MaximumLength(100).WithMessage("O email não pode ter mais que 100 caracteres");
    }
}
