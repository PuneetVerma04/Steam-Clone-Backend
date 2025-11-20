using FluentValidation;
using SteamClone.Backend.DTOs.User;

namespace SteamClone.Backend.Validators.User;

public class UpdateUserDtoValidator : AbstractValidator<UpdateUserDto>
{
    public UpdateUserDtoValidator()
    {
        When(x => x.Username != null, () =>
        {
            RuleFor(x => x.Username!)
                .ValidUsername();
        });

        When(x => x.Email != null, () =>
        {
            RuleFor(x => x.Email!)
                .ValidEmail();
        });

        When(x => x.Password != null, () =>
        {
            RuleFor(x => x.Password!)
                .StrongPassword();
        });
    }
}
