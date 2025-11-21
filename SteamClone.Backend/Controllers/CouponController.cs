using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.Entities;
using SteamClone.Backend.DTOs.Coupon;
using SteamClone.Backend.Services;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Manages promotional coupon operations including creation, retrieval, and deactivation
/// Uses EF Core for persistent database storage
/// </summary>
[ApiController]
[Route("store/[controller]")]
public class CouponController : ControllerBase
{
    private readonly ICouponService _couponService;

    /// <summary>
    /// Initializes the coupon controller with the coupon service
    /// </summary>
    public CouponController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    /// <summary>
    /// Retrieves all available coupons (Player and Admin access)
    /// </summary>
    /// <returns>Collection of all coupons including active and inactive</returns>
    [HttpGet]
    [Authorize(Roles = "Player,Admin")]
    public async Task<ActionResult<IEnumerable<CouponDto>>> GetCoupons()
    {
        var couponDtos = await _couponService.GetAllCouponsAsync();
        return Ok(couponDtos);
    }

    /// <summary>
    /// Retrieves a specific coupon by ID (Admin only)
    /// </summary>
    /// <param name="couponId">Coupon identifier</param>
    /// <returns>Coupon details if found, otherwise NotFound</returns>
    [HttpGet("{couponId}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CouponDto>> GetCouponById(int couponId)
    {
        var coupon = await _couponService.GetCouponByIdAsync(couponId);
        if (coupon == null)
        {
            return NotFound();
        }
        return Ok(coupon);
    }

    /// <summary>
    /// Creates a new promotional coupon (Admin only)
    /// </summary>
    /// <param name="newCouponDto">Coupon details including code, name, discount, and expiration</param>
    /// <returns>Created coupon with generated ID and location header</returns>
    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CouponDto>> CreateCoupon([FromBody] CreateCouponDto newCouponDto)
    {
        var createdCouponDto = await _couponService.CreateCouponAsync(newCouponDto);
        return CreatedAtAction(nameof(GetCouponById), new { couponId = createdCouponDto.CouponId }, createdCouponDto);
    }

    /// <summary>
    /// Deactivates a coupon, making it unusable (Admin only)
    /// </summary>
    /// <param name="couponId">Coupon identifier to deactivate</param>
    /// <returns>Deactivated coupon details, NotFound if doesn't exist, BadRequest if already inactive</returns>
    [HttpPatch("{couponId}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<CouponDto>> UpdateCoupon(int couponId)
    {
        var updatedCoupon = await _couponService.DeactivateCouponAsync(couponId);
        if (updatedCoupon == null)
        {
            return BadRequest("Coupon not found or already inactive");
        }

        return Ok(updatedCoupon);
    }
}