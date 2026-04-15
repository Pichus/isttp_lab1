using StudentParliamentSystem.Core.Aggregates.Statistics;

namespace StudentParliamentSystem.UseCases.Statistics.GetOverallStatistics;

public record GetOverallStatisticsQuery;

public class GetOverallStatisticsQueryHandler
{
    private readonly IStatisticsRepository _statisticsRepository;

    public GetOverallStatisticsQueryHandler(IStatisticsRepository statisticsRepository)
    {
        _statisticsRepository = statisticsRepository;
    }

    public async Task<OverallStatistics> Handle(GetOverallStatisticsQuery query, CancellationToken cancellationToken)
    {
        return await _statisticsRepository.GetOverallStatisticsAsync(cancellationToken);
    }
}