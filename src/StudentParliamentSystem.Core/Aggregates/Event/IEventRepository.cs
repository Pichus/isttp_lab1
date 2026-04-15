using StudentParliamentSystem.Core.Abstractions;

namespace StudentParliamentSystem.Core.Aggregates.Event;

public interface IEventRepository
{
    Task AddAsync(Event @event, CancellationToken cancellationToken = default);
    Task<Event?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<PagedResult<EventPreview>> RetrievePublishedAsync(
        int pageNumber, 
        int pageSize, 
        string? query, 
        string? tag, 
        string? sortOrder, 
        CancellationToken cancellationToken = default);
    Task<PagedResult<EventPreview>> RetrieveByDepartmentAsync(
        Guid departmentId,
        int pageNumber,
        int pageSize,
        string? query,
        CancellationToken cancellationToken = default);
    Task DeleteAsync(Event @event, CancellationToken cancellationToken = default);
    Task<IEnumerable<Event>> GetByCreatedByUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Event?> GetByIdWithRegistrationsAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddRegistrationAsync(EventRegistration registration, CancellationToken cancellationToken = default);
    Task RemoveRegistrationAsync(EventRegistration registration, CancellationToken cancellationToken = default);
}
