using FluentValidation;
using StudySummarizer.Application.DTOs;

namespace StudySummarizer.Application.Validators;

public class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage(ValidationMessages.EmailRequired)
            .EmailAddress().WithMessage(ValidationMessages.EmailInvalidFormat);

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage(ValidationMessages.PasswordRequired);
    }
}
