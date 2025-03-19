using FluentValidation;

namespace Application.UseCases.Persons.UpdatePerson;

public class UpdatePersonValidator : AbstractValidator<UpdatePersonCommand>
{
    public UpdatePersonValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("O nome é obrigatório")
            .MaximumLength(100).WithMessage("O nome não pode ter mais de 100 caracteres");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("O email é obrigatório")
            .EmailAddress().WithMessage("O email informado não é válido")
            .MaximumLength(100).WithMessage("O email não pode ter mais de 100 caracteres");
    }
}
