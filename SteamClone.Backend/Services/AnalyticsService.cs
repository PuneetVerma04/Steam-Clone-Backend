using SteamClone.Backend.DTOs.Analytics;
using SteamClone.Backend.Services.Interfaces;

namespace SteamClone.Backend.Services;

public class AnalyticsService : IAnalyticsService
{
    private readonly IGameService _gameService;
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public AnalyticsService(IGameService gameService, IUserService userService, IOrderService orderService)
    {
        _gameService = gameService;
        _userService = userService;
        _orderService = orderService;
    }

    public AnalyticsSummaryDto GetSummary()
    {
        var orders = _orderService.GetAllOrders().ToList();
        var users = _userService.GetAllUsers().ToList();

        return new AnalyticsSummaryDto
        {
            Date = DateTime.UtcNow.Date,
            TotalRevenue = orders.Sum(o => o.TotalPrice),
            TotalOrders = orders.Count,
            TotalUsers = users.Count
        };
    }

    public IEnumerable<TopGameDto> GetTopPurchasedGames(int count = 5)
    {
        var orders = _orderService.GetAllOrders();
        var games = _gameService.GetAllGames().ToList();

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

    public decimal GetRevenueLast30Days()
    {
        var cutoffDate = DateTime.UtcNow.AddDays(-30);
        return _orderService.GetAllOrders()
            .Where(o => o.OrderDate >= cutoffDate)
            .Sum(o => o.TotalPrice);
    }

    public IEnumerable<RevenueStatsDto> GetDailyRevenueStats(int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days).Date;
        var orders = _orderService.GetAllOrders()
            .Where(o => o.OrderDate >= startDate)
            .ToList();

        var dailyStats = new List<RevenueStatsDto>();
        for (int i = 0; i < days; i++)
        {
            var currentDate = startDate.AddDays(i);
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