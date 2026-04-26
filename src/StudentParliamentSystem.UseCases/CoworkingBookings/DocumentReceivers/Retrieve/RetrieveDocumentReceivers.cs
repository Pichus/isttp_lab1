using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.DocumentReceivers.Retrieve;

public record RetrieveDocumentReceiversQuery();

public class RetrieveDocumentReceiversQueryHandler
{
    private readonly IDocumentReceiverRepository _repository;

    public RetrieveDocumentReceiversQueryHandler(IDocumentReceiverRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<DocumentReceiver>> Handle(RetrieveDocumentReceiversQuery query, CancellationToken cancellationToken)
    {
        return await _repository.GetAllAsync(cancellationToken);
    }
}
