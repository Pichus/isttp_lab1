using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.CoworkingBookings.Request;

namespace StudentParliamentSystem.UnitTests.UseCases.CoworkingBookings;

public class RequestCoworkingBookingHandlerTests
{
    private readonly Mock<ICoworkingBookingRepository> _bookingRepo = new();
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RequestCoworkingBookingHandler _sut;

    public RequestCoworkingBookingHandlerTests()
    {
        _sut = new RequestCoworkingBookingHandler(_bookingRepo.Object, _eventRepo.Object, _uow.Object);
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFail()
    {
        var command = new RequestCoworkingBooking(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, DateTime.UtcNow.AddHours(1), null);
        _eventRepo.Setup(r => r.GetByIdAsync(command.EventId)).ReturnsAsync((Event?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Event not found");
    }

    [Fact]
    public async Task HandleAsync_NotEventOwner_ReturnsFail()
    {
        var ownerId = Guid.NewGuid();
        var requesterId = Guid.NewGuid();
        var @event = new Event { Id = Guid.NewGuid(), CreatedByUserId = ownerId };
        var command = new RequestCoworkingBooking(@event.Id, requesterId, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), null);
        
        _eventRepo.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("only book coworking for your own events");
    }

    [Fact]
    public async Task HandleAsync_StatusConfigError_ReturnsFail()
    {
        var ownerId = Guid.NewGuid();
        var @event = new Event { Id = Guid.NewGuid(), CreatedByUserId = ownerId };
        var command = new RequestCoworkingBooking(@event.Id, ownerId, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), null);
        
        _eventRepo.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Pending", It.IsAny<CancellationToken>())).ReturnsAsync((CoworkingBookingStatus?)null);

        var result = await _sut.HandleAsync(command);

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("status configuration error");
    }

    [Fact]
    public async Task HandleAsync_ValidRequest_CreatesBookingAndSaves()
    {
        var ownerId = Guid.NewGuid();
        var @event = new Event { Id = Guid.NewGuid(), CreatedByUserId = ownerId };
        var status = new CoworkingBookingStatus { Id = Guid.NewGuid(), Name = "Pending" };
        var command = new RequestCoworkingBooking(@event.Id, ownerId, DateTime.UtcNow, DateTime.UtcNow.AddHours(1), "Notes");
        
        _eventRepo.Setup(r => r.GetByIdAsync(@event.Id)).ReturnsAsync(@event);
        _bookingRepo.Setup(r => r.GetStatusByNameAsync("Pending", It.IsAny<CancellationToken>())).ReturnsAsync(status);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(command);

        result.IsSuccess.Should().BeTrue();
        _bookingRepo.Verify(r => r.AddAsync(It.Is<CoworkingBooking>(b => b.EventId == @event.Id && b.StatusId == status.Id && b.Notes == "Notes"), It.IsAny<CancellationToken>()), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }
}
