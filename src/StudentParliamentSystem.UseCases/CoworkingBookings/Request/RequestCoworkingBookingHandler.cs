using FluentResults;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.Event;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.Request;

public class RequestCoworkingBookingHandler
{
    private readonly ICoworkingBookingRepository _bookingRepository;
    private readonly IEventRepository _eventRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RequestCoworkingBookingHandler(
        ICoworkingBookingRepository bookingRepository,
        IEventRepository eventRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _eventRepository = eventRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RequestCoworkingBooking command)
    {
        var @event = await _eventRepository.GetByIdAsync(command.EventId);
        if (@event == null)
            return Result.Fail("Event not found");

        if (@event.CreatedByUserId != command.RequestingUserId)
            return Result.Fail("You can only book coworking for your own events");

        var pendingStatus = await _bookingRepository.GetStatusByNameAsync("Pending");
        if (pendingStatus == null)
            return Result.Fail("Booking status configuration error");

        var booking = new CoworkingBooking
        {
            Id = Guid.NewGuid(),
            EventId = command.EventId,
            StatusId = pendingStatus.Id,
            StartTimeUtc = command.StartTimeUtc,
            EndTimeUtc = command.EndTimeUtc,
            Notes = command.Notes
        };

        await _bookingRepository.AddAsync(booking);
        await _unitOfWork.SaveChangesAsync();

        return Result.Ok();
    }
}
