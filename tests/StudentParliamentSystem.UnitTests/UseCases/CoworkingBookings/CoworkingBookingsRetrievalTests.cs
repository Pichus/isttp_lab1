using Moq;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.CoworkingBookings.Retrieve;
using StudentParliamentSystem.UseCases.CoworkingBookings.Schedule;

namespace StudentParliamentSystem.UnitTests.UseCases.CoworkingBookings;

public class CoworkingBookingsRetrievalTests
{
    private readonly Mock<ICoworkingBookingRepository> _repo = new();

    [Fact]
    public async Task RetrieveCoworkingBookingsHandler_DelegatesToRepository()
    {
        var sut = new RetrieveCoworkingBookingsHandler(_repo.Object);
        var query = new RetrieveCoworkingBookings(2, 20, "Approved");
        var expected = new PagedResult<CoworkingBookingPreview> { Items = new List<CoworkingBookingPreview>(), TotalCount = 0, CurrentPage = 2, PageSize = 20 };
        
        _repo.Setup(r => r.GetAllAsync(2, 20, "Approved", It.IsAny<CancellationToken>()))
             .ReturnsAsync(expected);

        var result = await sut.HandleAsync(query);

        result.Should().Be(expected);
        _repo.Verify(r => r.GetAllAsync(2, 20, "Approved", It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetCoworkingScheduleHandler_DelegatesToRepository()
    {
        var sut = new GetCoworkingScheduleHandler(_repo.Object);
        var expected = new List<CoworkingBookingSlot>();
        
        _repo.Setup(r => r.GetScheduleAsync(It.IsAny<CancellationToken>()))
             .ReturnsAsync(expected);

        var result = await sut.HandleAsync(new GetCoworkingSchedule());

        result.Should().BeEquivalentTo(expected);
        _repo.Verify(r => r.GetScheduleAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
