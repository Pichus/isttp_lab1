using FluentResults;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.Approve;

public class ApproveCoworkingBookingHandler
{
    private readonly ICoworkingBookingRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveCoworkingBookingHandler(ICoworkingBookingRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result> HandleAsync(ApproveCoworkingBooking command)
    {
        var booking = await _repository.GetByIdAsync(command.BookingId);
        if (booking == null)
            return Result.Fail("Booking not found");

        var approvedStatus = await _repository.GetStatusByNameAsync("Approved");
        if (approvedStatus == null)
            return Result.Fail("Status configuration error");

        booking.StatusId = approvedStatus.Id;
        booking.SpaceManagerId = command.ManagerId;

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}
