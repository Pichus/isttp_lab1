namespace StudentParliamentSystem.Core.Aggregates.Statistics;

public interface IStatisticsRepository
{
    Task<OverallStatistics> GetOverallStatisticsAsync(CancellationToken cancellationToken = default);
}
