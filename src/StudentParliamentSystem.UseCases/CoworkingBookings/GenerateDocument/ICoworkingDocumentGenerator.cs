using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument;

public interface ICoworkingDocumentGenerator
{
    byte[] GenerateDocument(IEnumerable<CoworkingBooking> bookings, string receiver, string documentDate, string sender);
}