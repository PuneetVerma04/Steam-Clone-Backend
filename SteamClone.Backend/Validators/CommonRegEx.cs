namespace SteamClone.Backend.Validators;

public class CommonRegEx
{
    public const string UsernamePattern = "^[a-zA-Z0-9]{3,20}$";
    public const string GameTitlePattern = @"^[\w\s\-\.,:']{2,100}$";
    public const string CouponCodePattern = @"^[A-Z0-9]{4,20}$";
}