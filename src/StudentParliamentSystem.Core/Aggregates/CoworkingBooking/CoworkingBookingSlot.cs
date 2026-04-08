namespace StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

public record CoworkingBookingSlot(
    string EventTitle,
    DateTime StartTime,
    DateTime EndTime,
    string StatusName
);
