using SteamClone.Backend.DTOs.Analytics;
using SteamClone.Backend.Services.Interfaces;

namespace SteamClone.Backend.Services;

/// <summary>
/// Service for generating business analytics and reporting metrics
/// Provides insights into revenue, orders, users, and game performance
/// </summary>
public class AnalyticsService : IAnalyticsService
{
    private readonly IGameService _gameService;
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    /// <summary>
    /// Initializes the analytics service with dependent services
    /// </summary>
    public AnalyticsService(IGameService gameService, IUserService userService, IOrderService orderService)
    {
        _gameService = gameService;
        _userService = userService;
        _orderService = orderService;
    }

    /// <summary>
    /// Generates an overall business summary with key metrics
    /// </summary>
    /// <returns>Summary containing total revenue, orders, and users</returns>
    public async Task<AnalyticsSummaryDto> GetSummaryAsync()
    {
        var orders = (await _orderService.GetAllOrdersAsync()).ToList();
        var users = (await _userService.GetAllUsersAsync()).ToList();

        return new AnalyticsSummaryDto
        {
            Date = DateTime.UtcNow.Date,
            TotalRevenue = orders.Sum(o => o.TotalPrice),
            TotalOrders = orders.Count,
            TotalUsers = users.Count
        };
    }

    /// <summary>
    /// Identifies the most popular games by total purchases
    /// </summary>
    /// <param name="count">Number of top games to return</param>
    /// <returns>Collection of top games ranked by total purchases and revenue</returns>
    public async Task<IEnumerable<TopGameDto>> GetTopPurchasedGamesAsync(int count = 5)
    {
        var orders = await _orderService.GetAllOrdersAsync();
        var games = (await _gameService.GetAllGamesAsync()).ToList();

        // Aggregate order items by game and calculate totals
        var topGames = orders
            .SelectMany(o => o.Items)
            .GroupBy(i => i.GameId)
            .Select(g => new TopGameDto
            {
                Title = games.FirstOrDefault(x => x.Id == g.Key)?.Title ?? "Unknown",
                TotalPurchases = g.Sum(item => item.Quantity),
                TotalRevenue = g.Sum(item => item.Price * item.Quantity)
            })
            .OrderByDescending(g => g.TotalPurchases)
            .Take(count);

        return topGames;
    }

    /// <summary>
    /// Calculates total revenue generated in the last 30 days
    /// </summary>
    /// <returns>Sum of all order totals from the past 30 days</returns>
    public async Task<decimal> GetRevenueLast30DaysAsync()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        var orders = await _orderService.GetAllOrdersAsync();
        return orders
            .Where(o => o.OrderDate >= cutoffDate)
            .Sum(o => o.TotalPrice);
    }

    /// <summary>
    /// Generates daily revenue statistics for trend analysis and reporting
    /// </summary>
    /// <param name="days">Number of days to include in the analysis</param>
    /// <returns>Collection of daily revenue data points for the specified period</returns>
    public async Task<IEnumerable<RevenueStatsDto>> GetDailyRevenueStatsAsync(int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days).Date;
        var orders = (await _orderService.GetAllOrdersAsync())
            .Where(o => o.OrderDate >= startDate)
            .ToList();

        // Generate revenue data point for each day in the range
        var dailyStats = new List<RevenueStatsDto>();
        for (int i = 0; i < days; i++)
        {
            var currentDate = startDate.AddDays(i);

            // Sum all orders placed on this specific date
            var dayRevenue = orders
                .Where(o => o.OrderDate.Date == currentDate)
                .Sum(o => o.TotalPrice);

            dailyStats.Add(new RevenueStatsDto
            {
                Date = currentDate,
                DailyRevenue = dayRevenue
            });
        }

        return dailyStats;
    }
}