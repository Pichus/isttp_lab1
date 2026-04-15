using FluentResults;
using Moq;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;
using StudentParliamentSystem.UseCases.Events.Create;

namespace StudentParliamentSystem.UnitTests.UseCases.Events;

public class CreateEventHandlerTests
{
    private readonly Mock<IEventRepository> _eventRepo = new();
    private readonly Mock<IEventTagRepository> _tagRepo = new();
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly CreateEventHandler _sut;

    public CreateEventHandlerTests()
    {
        _sut = new CreateEventHandler(_eventRepo.Object, _tagRepo.Object, _uow.Object);
    }

    private static CreateEvent MakeCommand(string[]? tags = null) => new(
        "Title",
        "Description",
        "Location",
        DateTime.UtcNow.AddDays(1),
        DateTime.UtcNow.AddDays(1).AddHours(2),
        Guid.NewGuid(),
        50,
        true,
        tags?.ToList() ?? new List<string>(),
        Guid.NewGuid()
    );

    [Fact]
    public async Task HandleAsync_NoTags_CreatesEventAndSaves()
    {
        _eventRepo.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(MakeCommand());

        result.IsSuccess.Should().BeTrue();
        _eventRepo.Verify(r => r.AddAsync(It.Is<Event>(e => e.Title == "Title")), Times.Once);
        _uow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_ExistingTag_ReusesTag()
    {
        var tag = new EventTag { Id = Guid.NewGuid(), Name = "existing" };
        _tagRepo.Setup(r => r.GetByNameAsync("existing")).ReturnsAsync(tag);
        _eventRepo.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(MakeCommand(["existing"]));

        result.IsSuccess.Should().BeTrue();
        _tagRepo.Verify(r => r.AddAsync(It.IsAny<EventTag>()), Times.Never);
        _eventRepo.Verify(r => r.AddAsync(It.Is<Event>(e => e.Tags.Contains(tag))), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_NewTag_CreatesAndAttachesTag()
    {
        _tagRepo.Setup(r => r.GetByNameAsync("new-tag")).ReturnsAsync((EventTag?)null);
        _tagRepo.Setup(r => r.AddAsync(It.IsAny<EventTag>())).Returns(Task.CompletedTask);
        _eventRepo.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(MakeCommand(["new-tag"]));

        result.IsSuccess.Should().BeTrue();
        _tagRepo.Verify(r => r.AddAsync(It.Is<EventTag>(t => t.Name == "new-tag")), Times.Once);
    }

    [Fact]
    public async Task HandleAsync_BlankTagEntries_AreSkipped()
    {
        _eventRepo.Setup(r => r.AddAsync(It.IsAny<Event>())).Returns(Task.CompletedTask);
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>())).ReturnsAsync(1);

        var result = await _sut.HandleAsync(MakeCommand(["  ", "", "   "]));

        result.IsSuccess.Should().BeTrue();
        _tagRepo.Verify(r => r.GetByNameAsync(It.IsAny<string>()), Times.Never);
    }
}
