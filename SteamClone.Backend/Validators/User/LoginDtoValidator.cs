using FluentValidation;
using SteamClone.Backend.DTOs.User;

namespace SteamClone.Backend.Validators.User;

public class LoginDtoValidator : AbstractValidator<LoginDto>
{
    public LoginDtoValidator()
    {
        RuleFor(x => x.Email).ValidEmail();

        RuleFor(x => x.Password).NotEmpty().WithMessage("Password is required.");
    }
}