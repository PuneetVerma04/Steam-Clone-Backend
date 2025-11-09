using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Coupon;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace SteamClone.Backend.Services;

public class CouponService : ICouponService
{
    private readonly BackendDbContext _dbContext;
    private readonly IMapper _mapper;

    public CouponService(BackendDbContext dbContext, IMapper mapper)
    {
        _dbContext = dbContext;
        _mapper = mapper;
    }

    public IEnumerable<CouponDto> GetAllCoupons()
    {
        var coupons = _dbContext.Coupons.ToList();
        return _mapper.Map<IEnumerable<CouponDto>>(coupons);
    }

    public CouponDto? GetCouponById(int couponId)
    {
        var coupon = _dbContext.Coupons.Find(couponId);
        return coupon == null ? null : _mapper.Map<CouponDto>(coupon);
    }

    public CouponDto CreateCoupon(CreateCouponDto newCouponDto)
    {
        var newCoupon = _mapper.Map<Coupons>(newCouponDto);
        newCoupon.IsActive = true;
        newCoupon.CreatedAt = DateTime.UtcNow;

        _dbContext.Coupons.Add(newCoupon);
        _dbContext.SaveChanges();

        return _mapper.Map<CouponDto>(newCoupon);
    }

    public CouponDto? DeactivateCoupon(int couponId)
    {
        var coupon = _dbContext.Coupons.Find(couponId);
        if (coupon == null || !coupon.IsActive)
        {
            return null;
        }

        coupon.IsActive = false;
        coupon.ExpirationDate = DateTime.UtcNow;

        _dbContext.SaveChanges();

        return _mapper.Map<CouponDto>(coupon);
    }
}