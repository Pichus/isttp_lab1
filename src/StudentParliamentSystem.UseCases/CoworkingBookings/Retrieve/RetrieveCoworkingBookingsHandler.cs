using StudentParliamentSystem.Core.Abstractions;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.Retrieve;

public class RetrieveCoworkingBookingsHandler
{
    private readonly ICoworkingBookingRepository _repository;

    public RetrieveCoworkingBookingsHandler(ICoworkingBookingRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<CoworkingBookingPreview>> HandleAsync(RetrieveCoworkingBookings query)
    {
        return await _repository.GetAllAsync(query.PageNumber, query.PageSize, query.StatusFilter);
    }
}
