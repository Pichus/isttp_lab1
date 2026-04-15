using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Events.Register;

namespace StudentParliamentSystem.UnitTests.UseCases.Events;

public class RegisterForEventHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IUserRepository> _userRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly RegisterForEventHandler _sut;

    public RegisterForEventHandlerTests()
    {
        _sut = new RegisterForEventHandler(_eventRepo.Object, _userRepo.Object, _uow.Object);
    }

    private static User MakeUser(Guid? id = null) => User.Create(id ?? Guid.NewGuid(), "a@b.com", "A", "B");

    private static Event MakeEvent(bool published = true, int? maxParticipants = null, List<EventRegistration>? registrations = null) => new()
    {
        Id = Guid.NewGuid(),
        Title = "Test",
        Description = "Test",
        Location = "Loc",
        IsPublished = published,
        StartTimeUtc = DateTime.UtcNow.AddDays(1),
        EndTimeUtc = DateTime.UtcNow.AddDays(1).AddHours(2),
        CreatedByUserId = Guid.NewGuid(),
        DepartmentId = Guid.NewGuid(),
        CreatedAtUtc = DateTime.UtcNow,
        Tags = new List<EventTag>(),
        Registrations = registrations ?? new List<EventRegistration>(),
        MaxParticipants = maxParticipants
    };

    [Fact]
    public async Task HandleAsync_UserNotFound_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync((User?)null);

        var result = await _sut.HandleAsync(new RegisterForEvent(Guid.NewGuid(), userId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("not found");
    }

    [Fact]
    public async Task HandleAsync_EventNotFound_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var eventId = Guid.NewGuid();
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(MakeUser(userId));
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(eventId)).ReturnsAsync((Event?)null);

        var result = await _sut.HandleAsync(new RegisterForEvent(eventId, userId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("Event not found");
    }

    [Fact]
    public async Task HandleAsync_EventNotPublished_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var @event = MakeEvent(published: false);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(MakeUser(userId));
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);

        var result = await _sut.HandleAsync(new RegisterForEvent(@event.Id, userId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("unpublished");
    }

    [Fact]
    public async Task HandleAsync_AlreadyRegistered_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var reg = new EventRegistration { Id = Guid.NewGuid(), UserId = userId, EventId = Guid.NewGuid(), RegisteredAtUtc = DateTime.UtcNow };
        var @event = MakeEvent(registrations: [reg]);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(MakeUser(userId));
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);

        var result = await _sut.HandleAsync(new RegisterForEvent(@event.Id, userId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("already registered");
    }

    [Fact]
    public async Task HandleAsync_EventFull_ReturnsFail()
    {
        var userId = Guid.NewGuid();
        var existingReg = new EventRegistration { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), EventId = Guid.NewGuid(), RegisteredAtUtc = DateTime.UtcNow };
        var @event = MakeEvent(maxParticipants: 1, registrations: [existingReg]);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(MakeUser(userId));
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);

        var result = await _sut.HandleAsync(new RegisterForEvent(@event.Id, userId));

        result.IsFailed.Should().BeTrue();
        result.Errors[0].Message.Should().Contain("full");
    }

    [Fact]
    public async Task HandleAsync_ValidRegistration_ReturnsOkAndSaves()
    {
        var userId = Guid.NewGuid();
        var @event = MakeEvent();
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(MakeUser(userId));
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);
        _eventRepo.Setup(r => r.AddRegistrationAsync(It.IsAny<EventRegistration>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(new RegisterForEvent(@event.Id, userId));

        result.IsSuccess.Should().BeTrue();
        _eventRepo.Verify(r => r.AddRegistrationAsync(It.Is<EventRegistration>(reg => reg.UserId == userId && reg.EventId == @event.Id)), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NoMaxParticipants_AllowsUnlimitedRegistrations()
    {
        var userId = Guid.NewGuid();
        var @event = MakeEvent(maxParticipants: null);
        _userRepo.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(MakeUser(userId));
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(@event.Id)).ReturnsAsync(@event);
        _eventRepo.Setup(r => r.AddRegistrationAsync(It.IsAny<EventRegistration>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(new RegisterForEvent(@event.Id, userId));

        result.IsSuccess.Should().BeTrue();
    }
}
