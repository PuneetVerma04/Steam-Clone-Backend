using FluentValidation;
using SteamClone.Backend.DTOs.Game;

namespace SteamClone.Backend.Validators.Game;

public class UpdateGameRequestDTOValidator : AbstractValidator<UpdateGameRequestDTO>
{
    public UpdateGameRequestDTOValidator()
    {
        RuleFor(x => x.Id).ValidId();

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title!).ValidTitle();
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description!).ValidDescription();
        });

        When(x => x.Price != null, () =>
        {
            RuleFor(x => x.Price!.Value).ValidPrice();
        });

        When(x => x.Genre != null, () =>
        {
            RuleFor(x => x.Genre!)
                .NotEmpty().WithMessage("Genre cannot be empty.");
        });

        When(x => x.Publisher != null, () =>
        {
            RuleFor(x => x.Publisher!)
                .NotEmpty().WithMessage("Publisher cannot be empty.");
        });

        When(x => x.ReleaseDate != null, () =>
        {
            RuleFor(x => x.ReleaseDate!.Value).ValidReleaseDate();
        });

        When(x => x.ImageUrl != null, () =>
        {
            RuleFor(x => x.ImageUrl!).ValidUrl();
        });
    }
}
