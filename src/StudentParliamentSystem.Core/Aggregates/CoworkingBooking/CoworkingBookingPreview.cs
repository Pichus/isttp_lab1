namespace StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

public record CoworkingBookingPreview(
    Guid Id,
    string EventTitle,
    DateTime EventStart,
    DateTime EventEnd,
    DateTime StartTime,
    DateTime EndTime,
    string StatusName,
    string RequesterName,
    string? Notes
);
