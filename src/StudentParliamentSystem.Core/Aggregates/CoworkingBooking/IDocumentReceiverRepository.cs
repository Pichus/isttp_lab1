namespace StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

public interface IDocumentReceiverRepository
{
    Task<IEnumerable<DocumentReceiver>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<DocumentReceiver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(DocumentReceiver receiver, CancellationToken cancellationToken = default);
    Task UpdateAsync(DocumentReceiver receiver, CancellationToken cancellationToken = default);
    Task DeleteAsync(DocumentReceiver receiver, CancellationToken cancellationToken = default);
}
