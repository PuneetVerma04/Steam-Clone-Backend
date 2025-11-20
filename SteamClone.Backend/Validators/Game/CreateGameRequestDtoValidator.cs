using FluentValidation;
using SteamClone.Backend.DTOs.Game;

namespace SteamClone.Backend.Validators.Game;

public class CreateGameRequestDtoValidator : AbstractValidator<CreateGameRequestDTO>
{
    public CreateGameRequestDtoValidator()
    {
        RuleFor(x => x.Title).ValidTitle();

        RuleFor(x => x.Description).ValidDescription();

        RuleFor(x => x.Price).ValidPrice();

        RuleFor(x => x.Genre).NotEmpty().WithMessage("Genre is required.");

        RuleFor(x => x.Publisher).NotEmpty().WithMessage("Publisher is required.");

        RuleFor(x => x.ReleaseDate).ValidReleaseDate();

        RuleFor(x => x.ImageUrl).ValidUrl();
    }
}