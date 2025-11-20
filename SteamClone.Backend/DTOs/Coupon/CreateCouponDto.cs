namespace SteamClone.Backend.DTOs.Coupon;
public class CreateCouponDto
{
    
    public required string Code { get; set; }
    public required string CouponName { get; set; }
    public required int DiscountPercent { get; set; }
    public required DateTime ExpirationDate { get; set; }
}