namespace StudentParliamentSystem.UseCases.CoworkingBookings.Request;

public record RequestCoworkingBooking(
    Guid EventId,
    Guid RequestingUserId,
    DateTime StartTimeUtc,
    DateTime EndTimeUtc,
    string? Notes
);
