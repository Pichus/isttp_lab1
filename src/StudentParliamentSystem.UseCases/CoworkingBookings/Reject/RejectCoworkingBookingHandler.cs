using FluentResults;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.Reject;

public class RejectCoworkingBookingHandler
{
    private readonly ICoworkingBookingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public RejectCoworkingBookingHandler(ICoworkingBookingRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(RejectCoworkingBooking command)
    {
        var booking = await _repository.GetByIdAsync(command.BookingId);
        if (booking == null)
            return Result.Fail("Booking not found");

        var rejectedStatus = await _repository.GetStatusByNameAsync("Rejected");
        if (rejectedStatus == null)
            return Result.Fail("Status configuration error");

        booking.StatusId = rejectedStatus.Id;
        booking.SpaceManagerId = command.ManagerId;
        if (command.Notes != null)
            booking.Notes = command.Notes;

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}
