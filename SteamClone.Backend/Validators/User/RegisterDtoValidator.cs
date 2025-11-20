using FluentValidation;
using SteamClone.Backend.DTOs.User;

namespace SteamClone.Backend.Validators.User;

public class RegisterDtoValidator : AbstractValidator<RegisterDto>
{
    public RegisterDtoValidator()
    {
        RuleFor(x => x.Username).ValidUsername();
        RuleFor(x => x.Email).ValidEmail();
        RuleFor(x => x.Password).StrongPassword();
    }
}