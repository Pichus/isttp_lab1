using FluentResults;

using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Core.Aggregates.User;
using StudentParliamentSystem.UseCases.Abstractions;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.Approve;

public class ApproveCoworkingBookingHandler
{
    private readonly ICoworkingBookingRepository _repository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ApproveCoworkingBookingHandler(ICoworkingBookingRepository repository, IUserRepository userRepository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _userRepository = userRepository;
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

        var manager = await _userRepository.GetByIdAsync(command.ManagerId);
        if (manager == null)
            return Result.Fail("Invalid Space Manager selected.");

        booking.StatusId = approvedStatus.Id;
        booking.SpaceManagerId = manager.Id;

        await _unitOfWork.SaveChangesAsync();
        return Result.Ok();
    }
}