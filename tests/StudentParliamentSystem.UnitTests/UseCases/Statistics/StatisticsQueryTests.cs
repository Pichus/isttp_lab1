using Moq;
using StudentParliamentSystem.Core.Aggregates.Statistics;
using StudentParliamentSystem.UseCases.Statistics.GetOverallStatistics;

namespace StudentParliamentSystem.UnitTests.UseCases.Statistics;

public class StatisticsQueryTests
{
    private readonly Mock<IStatisticsRepository> _repo = new();

    [Fact]
    public async Task GetOverallStatisticsQueryHandler_DelegatesToRepository()
    {
        var sut = new GetOverallStatisticsQueryHandler(_repo.Object);
        var expected = new OverallStatistics(10, 5, 20, [], []);
        _repo.Setup(r => r.GetOverallStatisticsAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expected);

        var result = await sut.Handle(new GetOverallStatisticsQuery(), default);

        result.Should().Be(expected);
        _repo.Verify(r => r.GetOverallStatisticsAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
