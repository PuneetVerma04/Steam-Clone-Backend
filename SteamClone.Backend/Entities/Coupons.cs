namespace SteamClone.Backend.Entities;

/// <summary>
/// Represents a promotional discount coupon
/// </summary>
public class Coupons
{
    /// <summary>Unique identifier for the coupon</summary>
    public int CouponId { get; set; }

    /// <summary>Unique coupon code that users enter at checkout</summary>
    public required string Code { get; set; }

    /// <summary>Display name/description of the coupon</summary>
    public required string CouponName { get; set; }

    /// <summary>Percentage discount (e.g., 10 for 10% off)</summary>
    public required int DiscountPercent { get; set; }

    /// <summary>Whether the coupon is currently active and can be used</summary>
    public required bool IsActive { get; set; } = true;

    /// <summary>Timestamp when the coupon was created</summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>Date and time when the coupon expires and becomes unusable</summary>
    public required DateTime ExpirationDate { get; set; }
}