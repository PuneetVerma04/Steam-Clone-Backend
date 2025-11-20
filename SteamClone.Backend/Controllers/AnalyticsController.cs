using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SteamClone.Backend.Services.Interfaces;
using SteamClone.Backend.DTOs.Analytics;

namespace SteamClone.Backend.Controllers;

/// <summary>
/// Provides business analytics and reporting for administrators
/// Includes revenue statistics, user metrics, and top-performing games
/// </summary>
[ApiController]
[Route("store/[controller]")]
[Authorize(Roles = "Admin")]
public class AnalyticsController : ControllerBase
{
    private readonly IAnalyticsService _analyticsService;

    /// <summary>
    /// Initializes the analytics controller with analytics service
    /// </summary>
    public AnalyticsController(IAnalyticsService analyticsService)
    {
        _analyticsService = analyticsService;
    }

    /// <summary>
    /// Retrieves overall business summary including total revenue, orders, and users
    /// </summary>
    /// <returns>Summary analytics with key business metrics</returns>
    [HttpGet]
    public IActionResult GetSummary()
    {
        var summary = _analyticsService.GetSummary();
        return Ok(summary);
    }

    /// <summary>
    /// Retrieves the most purchased games ordered by total sales
    /// </summary>
    /// <param name="count">Number of top games to return (default: 5)</param>
    /// <returns>Collection of top games with purchase statistics</returns>
    [HttpGet("topGames")]
    public IActionResult GetTopPurchasedGames([FromQuery] int count = 5)
    {
        var topGames = _analyticsService.GetTopPurchasedGames(count);
        return Ok(topGames);
    }

    /// <summary>
    /// Calculates total revenue from the last 30 days
    /// </summary>
    /// <returns>Total revenue amount for the past 30 days</returns>
    [HttpGet("revenue")]
    public IActionResult GetRevenueLast30Days()
    {
        var revenue = _analyticsService.GetRevenueLast30Days();
        return Ok(new { Last30DaysRevenue = revenue });
    }

    /// <summary>
    /// Retrieves daily revenue breakdown for trend analysis
    /// </summary>
    /// <param name="days">Number of days to analyze (default: 30)</param>
    /// <returns>Daily revenue statistics for the specified period</returns>
    [HttpGet("revenue/daily")]
    public IActionResult GetDailyRevenueStats([FromQuery] int days = 30)
    {
        var dailyStats = _analyticsService.GetDailyRevenueStats(days);
        return Ok(dailyStats);
    }
}