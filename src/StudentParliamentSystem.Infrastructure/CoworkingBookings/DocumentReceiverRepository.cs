using Microsoft.EntityFrameworkCore;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;
using StudentParliamentSystem.Infrastructure.Data;

namespace StudentParliamentSystem.Infrastructure.CoworkingBookings;

public class DocumentReceiverRepository : IDocumentReceiverRepository
{
    private readonly ApplicationDatabaseContext _context;

    public DocumentReceiverRepository(ApplicationDatabaseContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<DocumentReceiver>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.DocumentReceivers
            .OrderByDescending(r => r.IsDefault)
            .ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
    }

    public async Task<DocumentReceiver?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.DocumentReceivers.FirstOrDefaultAsync(r => r.Id == id, cancellationToken);
    }

    public async Task AddAsync(DocumentReceiver receiver, CancellationToken cancellationToken = default)
    {
        if (receiver.IsDefault)
        {
            var existingDefaults = await _context.DocumentReceivers.Where(r => r.IsDefault).ToListAsync(cancellationToken);
            foreach (var d in existingDefaults) d.IsDefault = false;
        }
        await _context.DocumentReceivers.AddAsync(receiver, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(DocumentReceiver receiver, CancellationToken cancellationToken = default)
    {
        if (receiver.IsDefault)
        {
            var existingDefaults = await _context.DocumentReceivers.Where(r => r.IsDefault && r.Id != receiver.Id).ToListAsync(cancellationToken);
            foreach (var d in existingDefaults) d.IsDefault = false;
        }
        _context.DocumentReceivers.Update(receiver);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(DocumentReceiver receiver, CancellationToken cancellationToken = default)
    {
        _context.DocumentReceivers.Remove(receiver);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
