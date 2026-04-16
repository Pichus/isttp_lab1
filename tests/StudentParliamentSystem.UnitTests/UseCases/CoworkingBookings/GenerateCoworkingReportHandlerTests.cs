using Moq;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument;

namespace StudentParliamentSystem.UnitTests.UseCases.CoworkingBookings;

public class GenerateCoworkingReportHandlerTests
{
    private readonly Mock<ICoworkingBookingRepository> _repo = new();
    private readonly Mock<ICoworkingDocumentGenerator> _generator = new();
    private readonly GenerateCoworkingReportHandler _sut;

    public GenerateCoworkingReportHandlerTests()
    {
        _sut = new GenerateCoworkingReportHandler(_repo.Object, _generator.Object);
    }

    [Fact]
    public async Task HandleAsync_EndBeforeStart_ReturnsFail()
    {
        var start = DateTime.UtcNow.AddDays(2);
        var end = DateTime.UtcNow.AddDays(1);

        var result = await _sut.HandleAsync(new GenerateCoworkingReport(start, end));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("End date must be after start date");
    }

    [Fact]
    public async Task HandleAsync_EndEqualToStart_ReturnsFail()
    {
        var dt = DateTime.UtcNow;

        var result = await _sut.HandleAsync(new GenerateCoworkingReport(dt, dt));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("End date must be after start date");
    }

    [Fact]
    public async Task HandleAsync_SpanExceedsOneWeek_ReturnsFail()
    {
        var start = DateTime.UtcNow;
        var end = start.AddDays(8);

        var result = await _sut.HandleAsync(new GenerateCoworkingReport(start, end));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("one week");
    }

    [Fact]
    public async Task HandleAsync_ValidSpan_FetchesBookingsAndGeneratesDocument()
    {
        var start = DateTime.UtcNow;
        var end = start.AddDays(3);
        var bookings = new List<CoworkingBooking> { new() { Id = Guid.NewGuid() } };
        var docBytes = new byte[] { 1, 2, 3 };

        _repo.Setup(r => r.GetApprovedBookingsWithinSpanAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(bookings);
        _generator.Setup(g => g.GenerateDocument(bookings, It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(docBytes);

        var result = await _sut.HandleAsync(new GenerateCoworkingReport(start, end));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().BeEquivalentTo(docBytes);
        _repo.Verify(r => r.GetApprovedBookingsWithinSpanAsync(
            It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()), Times.Once);
        _generator.Verify(g => g.GenerateDocument(bookings, It.IsAny<DateTime>(), It.IsAny<DateTime>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ExactlyOneWeekSpan_Succeeds()
    {
        var start = DateTime.UtcNow;
        var end = start.AddDays(7);
        var docBytes = Array.Empty<byte>();

        _repo.Setup(r => r.GetApprovedBookingsWithinSpanAsync(
                It.IsAny<DateTime>(), It.IsAny<DateTime>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<CoworkingBooking>());
        _generator.Setup(g => g.GenerateDocument(It.IsAny<IEnumerable<CoworkingBooking>>(),
                It.IsAny<DateTime>(), It.IsAny<DateTime>()))
            .Returns(docBytes);

        var result = await _sut.HandleAsync(new GenerateCoworkingReport(start, end));

        result.IsSuccess.Should().BeTrue();
    }
}
