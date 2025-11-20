using FluentValidation;

namespace SteamClone.Backend.Validators;

public static class CentralValidator
{
  public static IRuleBuilderOptions<T, string> ValidEmail<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .EmailAddress().WithMessage(ValidationMessages.InvalidEmail);
  }

  public static IRuleBuilderOptions<T, string> ValidUsername<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .Matches(CommonRegEx.UsernamePattern).WithMessage(ValidationMessages.InvalidUsername);
  }

  public static IRuleBuilderOptions<T, string> StrongPassword<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .MinimumLength(8)
        .Matches("[A-Z]").WithMessage(ValidationMessages.PasswordTooWeak)
        .Matches("[a-z]").WithMessage(ValidationMessages.PasswordTooWeak)
        .Matches("[0-9]").WithMessage(ValidationMessages.PasswordTooWeak);
  }

  public static IRuleBuilderOptions<T, int> ValidQuantity<T>(this IRuleBuilder<T, int> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .GreaterThan(0).WithMessage(ValidationMessages.InvalidQuantity);
  }

  public static IRuleBuilderOptions<T, decimal> ValidPrice<T>(this IRuleBuilder<T, decimal> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .GreaterThanOrEqualTo(0).WithMessage(ValidationMessages.InvalidPrice);
  }

  public static IRuleBuilderOptions<T, int> ValidRating<T>(this IRuleBuilder<T, int> rule)
  {
    return rule
        .InclusiveBetween(1, 5).WithMessage(ValidationMessages.InvalidRating);
  }

  public static IRuleBuilderOptions<T, string> ValidTitle<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .MaximumLength(100).WithMessage(ValidationMessages.TitleMaxLength)
        .Matches(CommonRegEx.GameTitlePattern).WithMessage(ValidationMessages.TitleContainsInvalidCharacters);
  }

  public static IRuleBuilderOptions<T, string> ValidDescription<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .NotEmpty().WithMessage(ValidationMessages.RequiredField)
        .MaximumLength(1000).WithMessage(ValidationMessages.DescriptionMaxLength);
  }

  public static IRuleBuilderOptions<T, DateTime> ValidReleaseDate<T>(this IRuleBuilder<T, DateTime> rule)
  {
    return rule
        .LessThanOrEqualTo(DateTime.UtcNow)
        .WithMessage(ValidationMessages.InvalidReleaseDate);
  }

  public static IRuleBuilderOptions<T, int> ValidId<T>(this IRuleBuilder<T, int> rule)
  {
    return rule
        .GreaterThan(0)
        .WithMessage(ValidationMessages.InvalidId);
  }

  public static IRuleBuilderOptions<T, string> ValidUrl<T>(this IRuleBuilder<T, string> rule)
  {
    return rule
        .Must(url => Uri.IsWellFormedUriString(url, UriKind.Absolute))
               .WithMessage(ValidationMessages.InvalidImageUrl);
  }
}