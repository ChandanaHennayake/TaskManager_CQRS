using FluentValidation;

namespace TaskManager.Application.Features.Users.Queries.Login;

public class LoginQueryValidator
    : AbstractValidator<LoginQuery>
{
    public LoginQueryValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);
    }
}