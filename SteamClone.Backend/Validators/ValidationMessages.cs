namespace SteamClone.Backend.Validators;

public class ValidationMessages
{
    public const string RequiredField = "This field is required.";
    public const string TitleMaxLength = "Title cannot exceed 100 characters.";
    public const string DescriptionMaxLength = "Description cannot exceed 1000 characters.";
    public const string TitleContainsInvalidCharacters = "Title contains invalid characters.";
    public const string InvalidEmail = "The email format is invalid.";
    public const string InvalidReleaseDate = "Release date cannot be in the future.";
    public const string InvalidUsername = "Username can only contain alphanumeric characters and underscores.";
    public const string InvalidImageUrl = "ImageUrl must be a valid URL.";
    public const string PasswordTooWeak = "Password must be at least 8 characters long and include uppercase, lowercase, number, and special character.";
    public const string PasswordsDoNotMatch = "Passwords do not match.";
    public const string InvalidId = "Invalid ID provided.";
    public const string InvalidPrice = "The price must be greater than or equal to zero.";
    public const string InvalidQuantity = "The quantity must be at least 1.";
    public const string InvalidRating = "Rating must be between 1 and 5.";
}