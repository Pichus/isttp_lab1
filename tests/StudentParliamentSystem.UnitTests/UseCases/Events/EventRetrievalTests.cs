using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Events.Retrieve.ByDepartment;
using StudentParliamentSystem.UseCases.Events.Retrieve.ById;
using StudentParliamentSystem.UseCases.Events.Retrieve.CreatedByUser;
using StudentParliamentSystem.UseCases.Events.Retrieve.Published;
using StudentParliamentSystem.UseCases.Events.Retrieve.Tags;

namespace StudentParliamentSystem.UnitTests.UseCases.Events;

public class EventRetrievalTests
{
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IEventTagRepository> _tagRepo = new();

    [Fact]
    public async Task RetrieveAllEventTagsHandler_DelegatesToRepository()
    {
        var sut = new RetrieveAllEventTagsHandler(_tagRepo.Object);
        var expected = new List<EventTag>();
        _tagRepo.Setup(r => r.RetrieveAllAsync()).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrieveAllEventTags());

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task RetrieveDepartmentEventsHandler_DelegatesToRepository()
    {
        var sut = new RetrieveDepartmentEventsHandler(_eventRepo.Object);
        var deptId = Guid.NewGuid();
        var expected = new PagedResult<EventPreview> { Items = new List<EventPreview>(), TotalCount = 0, CurrentPage = 1, PageSize = 10 };
        _eventRepo.Setup(r => r.RetrieveByDepartmentAsync(deptId, 1, 10, "q")).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrieveDepartmentEvents(deptId, 1, 10, "q"));

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task RetrieveEventByIdHandler_Found_ReturnsOk()
    {
        var sut = new RetrieveEventByIdHandler(_eventRepo.Object);
        var id = Guid.NewGuid();
        var @event = new Event { Id = id };
        _eventRepo.Setup(r => r.GetByIdAsync(id)).ReturnsAsync(@event);

        var result = await sut.HandleAsync(new RetrieveEventById(id));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(@event);
    }

    [Fact]
    public async Task RetrieveEventByIdHandler_NotFound_ReturnsFail()
    {
        var sut = new RetrieveEventByIdHandler(_eventRepo.Object);
        _eventRepo.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Event?)null);

        var result = await sut.HandleAsync(new RetrieveEventById(Guid.NewGuid()));

        result.IsFailed.Should().BeTrue();
    }

    [Fact]
    public async Task RetrieveEventByIdWithRegistrationsHandler_DelegatesToRepository()
    {
        var sut = new RetrieveEventByIdWithRegistrationsHandler(_eventRepo.Object);
        var id = Guid.NewGuid();
        var @event = new Event { Id = id };
        _eventRepo.Setup(r => r.GetByIdWithRegistrationsAsync(id)).ReturnsAsync(@event);

        var result = await sut.HandleAsync(new RetrieveEventByIdWithRegistrations(id));

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().Be(@event);
    }

    [Fact]
    public async Task RetrieveEventsCreatedByUserHandler_DelegatesToRepository()
    {
        var sut = new RetrieveEventsCreatedByUserHandler(_eventRepo.Object);
        var userId = Guid.NewGuid();
        var expected = new List<Event>();
        _eventRepo.Setup(r => r.GetByCreatedByUserAsync(userId)).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrieveEventsCreatedByUser(userId));

        result.Should().BeSameAs(expected);
    }

    [Fact]
    public async Task RetrievePublishedEventsHandler_DelegatesToRepository()
    {
        var sut = new RetrievePublishedEventsHandler(_eventRepo.Object);
        var expected = new PagedResult<EventPreview> { Items = new List<EventPreview>(), TotalCount = 0, CurrentPage = 1, PageSize = 10 };
        _eventRepo.Setup(r => r.RetrievePublishedAsync(1, 10, "q", "t", "sort")).ReturnsAsync(expected);

        var result = await sut.HandleAsync(new RetrievePublishedEvents(1, 10, "q", "t", "sort"));

        result.Should().BeSameAs(expected);
    }
}
