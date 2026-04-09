namespace StudentParliamentSystem.UseCases.CoworkingBookings.Reject;

public record RejectCoworkingBooking(Guid BookingId, Guid ManagerId, string? Notes);
