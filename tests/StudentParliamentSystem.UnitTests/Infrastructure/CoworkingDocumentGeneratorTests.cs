using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.Infrastructure.CoworkingBookings;

namespace StudentParliamentSystem.UnitTests.Infrastructure;

public class CoworkingDocumentGeneratorTests
{
    private readonly CoworkingDocumentGenerator _sut = new();

    [Fact]
    public void GenerateDocument_NoBookings_GeneratesMessage()
    {
        var spanStart = DateTime.UtcNow;
        var spanEnd = DateTime.UtcNow.AddDays(7);
        
        var bytes = _sut.GenerateDocument(new List<CoworkingBooking>(), spanStart, spanEnd);

        bytes.Should().NotBeNullOrEmpty();
        
        using var ms = new MemoryStream(bytes);
        using var doc = WordprocessingDocument.Open(ms, false);
        var text = doc.MainDocumentPart!.Document.Body!.InnerText;
        text.Should().Contain("заходів не заплановано");
    }

    [Fact]
    public void GenerateDocument_WithBookings_IncludesBookingDetails()
    {
        var startTime = new DateTime(2026, 5, 20, 10, 0, 0, DateTimeKind.Utc);
        var endTime = startTime.AddHours(2);
        
        var booking = new CoworkingBooking
        {
            Id = Guid.NewGuid(),
            StartTimeUtc = startTime,
            EndTimeUtc = endTime,
            Event = new Event
            {
                Title = "Test Event",
                CreatedByUser = User.Create(Guid.NewGuid(), "a@b.com", "Ivan", "Ivanov"),
                EventOrganizers = new List<EventOrganizer>()
            },
            SpaceManager = User.Create(Guid.NewGuid(), "m@b.com", "Petro", "Petrov")
        };

        var bytes = _sut.GenerateDocument(new List<CoworkingBooking> { booking }, startTime.AddDays(-1), startTime.AddDays(1));

        bytes.Should().NotBeNullOrEmpty();
        
        using var ms = new MemoryStream(bytes);
        using var doc = WordprocessingDocument.Open(ms, false);
        var text = doc.MainDocumentPart!.Document.Body!.InnerText;
        
        text.Should().Contain("Test Event");
        text.Should().Contain("Ivanov Ivan");
        text.Should().Contain("Petrov Petro");
    }
}
