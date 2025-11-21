using SteamClone.Backend.DTOs.Analytics;

namespace SteamClone.Backend.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task<AnalyticsSummaryDto> GetSummaryAsync();
        Task<IEnumerable<TopGameDto>> GetTopPurchasedGamesAsync(int count = 5);
        Task<decimal> GetRevenueLast30DaysAsync();
        Task<IEnumerable<RevenueStatsDto>> GetDailyRevenueStatsAsync(int days = 30);
    }
}
