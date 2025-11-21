using FluentValidation;
using SteamClone.Backend.DTOs.Game;
using SteamClone.Backend.Entities;

namespace SteamClone.Backend.Validators.Game;

public class CreateGameRequestDtoValidator : AbstractValidator<CreateGameRequestDTO>
{
    private readonly BackendDbContext _dbContext;

    public CreateGameRequestDtoValidator(BackendDbContext dbContext)
    {
        _dbContext = dbContext;

        RuleFor(x => x.Title).ValidTitle();

        RuleFor(x => x.Description).ValidDescription();

        RuleFor(x => x.Price).ValidPrice();

        RuleFor(x => x.Genre).NotEmpty().WithMessage("Genre is required.");

        RuleFor(x => x.PublisherId)
            .Must(BeAValidPublisher).WithMessage("PublisherId must reference a user with Publisher role.");

        RuleFor(x => x.ReleaseDate).ValidReleaseDate();

        RuleFor(x => x.ImageUrl).ValidUrl();
    }

    private bool BeAValidPublisher(int publisherId)
    {
        return _dbContext.Users.Any(u => u.Id == publisherId && u.Role == UserRole.Publisher);
    }
}