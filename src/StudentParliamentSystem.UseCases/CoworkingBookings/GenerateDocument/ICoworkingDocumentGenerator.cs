using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument;

public interface ICoworkingDocumentGenerator
{
    byte[] GenerateDocument(IEnumerable<CoworkingBooking> bookings, DateTime spanStart, DateTime spanEnd);
}