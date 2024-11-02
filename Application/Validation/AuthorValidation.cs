using Domain.Entities;
using FluentValidation;

namespace Application.Validation;

public class AuthorValidation : AbstractValidator<Author>
{
    public AuthorValidation()
    {
        RuleFor(a => a.FullName).NotNull()
            .NotEmpty();

        RuleFor(a=>a.BirthDate).NotEmpty()
             .NotNull()
             .Must(x => x < DateOnly.FromDateTime(DateTime.Now).AddYears(-5))
             .WithMessage("Date cannot small 5 age");

    }
}
