namespace StudentParliamentSystem.Core.Aggregates.Statistics;

public interface IStatisticsRepository
{
    Task<OverallStatistics> GetOverallStatisticsAsync(DateTime? startDate = null, DateTime? endDate = null, CancellationToken cancellationToken = default);
}