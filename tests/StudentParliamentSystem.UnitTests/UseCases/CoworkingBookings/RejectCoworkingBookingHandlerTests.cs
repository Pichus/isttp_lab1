using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.CoworkingBookings.Reject;

namespace StudentParliamentSystem.UnitTests.UseCases.CoworkingBookings;

public class RejectCoworkingBookingHandlerTests
{
    private readonly Mock<ICoworkingBookingRepository> _bookingRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RejectCoworkingBookingHandler _sut;

    public RejectCoworkingBookingHandlerTests()
    {
        _sut = new RejectCoworkingBookingHandler(_bookingRepo.Object, _uow.Object);
    }

    [Fact]
    public async Task HandleAsync_BookingNotFound_ReturnsFail()
    {
        var command = new RejectCoworkingBooking(Guid.NewGuid(), Guid.NewGuid(), "Notes");
        _bookingRepo.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync((CoworkingBooking?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Booking not found");
    }

    [Fact]
    public async Task HandleAsync_StatusConfigError_ReturnsFail()
    {
        var booking = new CoworkingBooking { Id = Guid.NewGuid() };
        var command = new RejectCoworkingBooking(booking.Id, Guid.NewGuid(), "Notes");
        
        _bookingRepo.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Rejected", It.IsAny<CancellationToken>())).ReturnsAsync((CoworkingBookingStatus?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Status configuration error");
    }

    [Fact]
    public async Task HandleAsync_ValidRejection_UpdatesStatusAndNotes()
    {
        var booking = new CoworkingBooking { Id = Guid.NewGuid() };
        var status = new CoworkingBookingStatus { Id = Guid.NewGuid(), Name = "Rejected" };
        var managerId = Guid.NewGuid();
        var command = new RejectCoworkingBooking(booking.Id, managerId, "Reject reason");
        
        _bookingRepo.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Rejected", It.IsAny<CancellationToken>())).ReturnsAsync(status);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        booking.StatusId.Should().Be(status.Id);
        booking.SpaceManagerId.Should().Be(managerId);
        booking.Notes.Should().Be("Reject reason");
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
