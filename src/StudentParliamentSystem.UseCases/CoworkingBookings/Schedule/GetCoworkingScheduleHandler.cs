using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.Schedule;

public class GetCoworkingScheduleHandler
{
    private readonly ICoworkingBookingRepository _repository;

    public GetCoworkingScheduleHandler(ICoworkingBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<CoworkingBookingSlot>> HandleAsync(GetCoworkingSchedule query)
    {
        return await _repository.GetScheduleAsync();
    }
}
