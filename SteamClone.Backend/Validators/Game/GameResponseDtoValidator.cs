using System.Data;
using FluentValidation;
using SteamClone.Backend.DTOs.Game;

namespace SteamClone.Backend.Validators.Game;

public class GameResponseDtoValidator : AbstractValidator<GameResponseDTO>
{
    public GameResponseDtoValidator()
    {
        RuleFor(x => x.Id)
            .GreaterThan(0)
            .WithMessage("Invalid game ID.");

        When(x => x.Title != null, () =>
        {
            RuleFor(x => x.Title!)
                .NotEmpty().WithMessage("Title cannot be empty.")
                .MaximumLength(100).WithMessage("Title cannot exceed 100 characters.");
        });

        When(x => x.Description != null, () =>
        {
            RuleFor(x => x.Description!)
                .NotEmpty().WithMessage("Description cannot be empty.")
                .MaximumLength(1000).WithMessage("Description cannot exceed 1000 characters.");
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

        When(x => x.ImageUrl != null, () =>
        {
            RuleFor(x => x.ImageUrl!)
                .NotEmpty().WithMessage("ImageUrl cannot be empty.")
                .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("ImageUrl must be a valid URL.");
        });

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price must be non-negative.");

        RuleFor(x => x.ReleaseDate)
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Release date cannot be in the future.");
    }
}