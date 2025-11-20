using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Coupon;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

/// <summary>
/// Service for managing promotional coupons including creation, retrieval, and deactivation
/// </summary>
public class CouponService : ICouponService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes the coupon service with database context and AutoMapper
    /// </summary>
    public CouponService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    /// <summary>
    /// Retrieves all coupons from the database
    /// </summary>
    /// <returns>Collection of all coupon DTOs including active and inactive</returns>
    public IEnumerable<CouponDto> GetAllCoupons()
    {
        var coupons = _dbContext.Coupons.ToList();
        return _mapper.Map<IEnumerable<CouponDto>>(coupons);
    }

    /// <summary>
    /// Retrieves a specific coupon by its identifier
    /// </summary>
    /// <param name="couponId">Coupon ID</param>
    /// <returns>Coupon DTO if found, null otherwise</returns>
    public CouponDto? GetCouponById(int couponId)
    {
        var coupon = _dbContext.Coupons.Find(couponId);
        return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
    }

    /// <summary>
    /// Creates a new promotional coupon
    /// </summary>
    /// <param name="newCouponDto">Coupon details including code, discount percentage, and expiration</param>
    /// <returns>Created coupon DTO with generated ID</returns>
    public CouponDto CreateCoupon(CreateCouponDto newCouponDto)
    {
        var newCoupon = _mapper.Map<Coupons>(newCouponDto);

        // Set default values for new coupon
        newCoupon.IsActive = true;
        newCoupon.CreatedAt = DateTime.UtcNow;

        _dbContext.Coupons.Add(newCoupon);
        _dbContext.SaveChanges();

        return _mapper.Map<CouponDto>(newCoupon);
    }

    /// <summary>
    /// Deactivates a coupon, making it unusable for future purchases
    /// </summary>
    /// <param name="couponId">Coupon ID to deactivate</param>
    /// <returns>Deactivated coupon DTO if successful, null if not found or already inactive</returns>
    public CouponDto? DeactivateCoupon(int couponId)
    {
        var coupon = _dbContext.Coupons.Find(couponId);

        // Validate coupon exists and is currently active
        if (coupon == null || !coupon.IsActive)
        {
            return null;
        }

        // Mark coupon as inactive and set expiration to current time
        coupon.IsActive = false;
        coupon.ExpirationDate = DateTime.UtcNow;

        _dbContext.SaveChanges();

        return _mapper.Map<CouponDto>(coupon);
    }
}