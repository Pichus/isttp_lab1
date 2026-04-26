using FluentResults;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.DocumentReceivers.Delete;

public record DeleteDocumentReceiverCommand(Guid Id);

public class DeleteDocumentReceiverCommandHandler
{
    private readonly IDocumentReceiverRepository _repository;

    public DeleteDocumentReceiverCommandHandler(IDocumentReceiverRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(DeleteDocumentReceiverCommand command, CancellationToken cancellationToken)
    {
        var receiver = await _repository.GetByIdAsync(command.Id, cancellationToken);
        if (receiver == null) return Result.Fail("Receiver not found");

        await _repository.DeleteAsync(receiver, cancellationToken);
        return Result.Ok();
    }
}
