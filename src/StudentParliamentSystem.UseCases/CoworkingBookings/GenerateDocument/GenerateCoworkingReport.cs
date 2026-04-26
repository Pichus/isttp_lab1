using FluentResults;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.GenerateDocument;

public record GenerateCoworkingReport(DateTime Start, DateTime End, string Receiver, string DocumentDate, string Sender);

public class GenerateCoworkingReportHandler
{
    private readonly ICoworkingBookingRepository _repository;
    private readonly ICoworkingDocumentGenerator _documentGenerator;

    public GenerateCoworkingReportHandler(ICoworkingBookingRepository repository, ICoworkingDocumentGenerator documentGenerator)
    {
        _repository = repository;
        _documentGenerator = documentGenerator;
    }

    public async Task<Result<byte[]>> HandleAsync(GenerateCoworkingReport query)
    {
        var startUtc = query.Start.ToUniversalTime();
        var endUtc = query.End.ToUniversalTime();

        if (endUtc <= startUtc)
        {
            return Result.Fail("End date must be after start date.");
        }

        var bookings = await _repository.GetApprovedBookingsWithinSpanAsync(startUtc, endUtc);

        var docBytes = _documentGenerator.GenerateDocument(bookings, query.Receiver, query.DocumentDate, query.Sender);

        return Result.Ok(docBytes);
    }
}