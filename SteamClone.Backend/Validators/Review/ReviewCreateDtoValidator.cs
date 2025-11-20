using FluentValidation;
using SteamClone.Backend.DTOs.Review;
using SteamClone.Backend.Validators;

public class ReviewCreateDtoValidator : AbstractValidator<ReviewCreateDto>
{
  public ReviewCreateDtoValidator()
  {
    RuleFor(x => x.UserId)
        .ValidId();

    RuleFor(x => x.GameId)
        .ValidId();

    RuleFor(x => x.Comment)
        .MaximumLength(500).WithMessage("Comment too long.")
        .When(x => x.Comment != null);

    RuleFor(x => x.Rating)
        .ValidRating();
  }
}
