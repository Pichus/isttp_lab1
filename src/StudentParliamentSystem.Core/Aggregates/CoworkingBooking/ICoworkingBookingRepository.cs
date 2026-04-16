using StudentParliamentSystem.Core.Abstractions;

namespace StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

public interface ICoworkingBookingRepository
{
    Task AddAsync(CoworkingBooking booking, CancellationToken cancellationToken = default);
    Task<CoworkingBooking?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<CoworkingBookingPreview>> GetAllAsync(int pageNumber, int pageSize, string? statusFilter, CancellationToken cancellationToken = default);
    Task<IEnumerable<CoworkingBookingSlot>> GetScheduleAsync(CancellationToken cancellationToken = default);
    Task<CoworkingBookingStatus?> GetStatusByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<IEnumerable<CoworkingBooking>> GetApprovedBookingsWithinSpanAsync(DateTime startUtc, DateTime endUtc, CancellationToken cancellationToken = default);
}