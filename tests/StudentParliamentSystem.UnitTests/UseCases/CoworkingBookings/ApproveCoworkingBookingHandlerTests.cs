using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.CoworkingBookings.Approve;

namespace StudentParliamentSystem.UnitTests.UseCases.CoworkingBookings;

public class ApproveCoworkingBookingHandlerTests
{
    private readonly Mock<ICoworkingBookingRepository> _bookingRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly ApproveCoworkingBookingHandler _sut;

    public ApproveCoworkingBookingHandlerTests()
    {
        _sut = new ApproveCoworkingBookingHandler(_bookingRepo.Object, _userRepo.Object, _uow.Object);
    }

    [Fact]
    public async Task HandleAsync_BookingNotFound_ReturnsFail()
    {
        var command = new ApproveCoworkingBooking(Guid.NewGuid(), Guid.NewGuid());
        _bookingRepo.Setup(r => r.GetByIdAsync(command.BookingId, It.IsAny<CancellationToken>())).ReturnsAsync((CoworkingBooking?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Booking not found");
    }

    [Fact]
    public async Task HandleAsync_StatusConfigError_ReturnsFail()
    {
        var booking = new CoworkingBooking { Id = Guid.NewGuid() };
        var command = new ApproveCoworkingBooking(booking.Id, Guid.NewGuid());
        
        _bookingRepo.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Approved", It.IsAny<CancellationToken>())).ReturnsAsync((CoworkingBookingStatus?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Status configuration error");
    }

    [Fact]
    public async Task HandleAsync_ManagerNotFound_ReturnsFail()
    {
        var booking = new CoworkingBooking { Id = Guid.NewGuid() };
        var status = new CoworkingBookingStatus { Id = Guid.NewGuid(), Name = "Approved" };
        var command = new ApproveCoworkingBooking(booking.Id, Guid.NewGuid());
        
        _bookingRepo.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Approved", It.IsAny<CancellationToken>())).ReturnsAsync(status);
        _userRepo.Setup(r => r.GetByIdAsync(command.ManagerId)).ReturnsAsync((User?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Invalid Space Manager");
    }

    [Fact]
    public async Task HandleAsync_ValidApproval_UpdatesStatusAndManager()
    {
        var booking = new CoworkingBooking { Id = Guid.NewGuid() };
        var status = new CoworkingBookingStatus { Id = Guid.NewGuid(), Name = "Approved" };
        var manager = User.Create(Guid.NewGuid(), "a@b.com", "A", "B");
        var command = new ApproveCoworkingBooking(booking.Id, manager.Id);
        
        _bookingRepo.Setup(r => r.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Approved", It.IsAny<CancellationToken>())).ReturnsAsync(status);
        _userRepo.Setup(r => r.GetByIdAsync(manager.Id)).ReturnsAsync(manager);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        booking.StatusId.Should().Be(status.Id);
        booking.SpaceManagerId.Should().Be(manager.Id);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
