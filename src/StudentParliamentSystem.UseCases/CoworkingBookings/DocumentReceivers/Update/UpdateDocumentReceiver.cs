using FluentResults;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

using Wolverine.Shims.MediatR;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.DocumentReceivers.Update;

public record UpdateDocumentReceiver(Guid Id, string Name, string Position, string FullTitle, bool IsDefault) : IRequest<Result>;

public class UpdateDocumentReceiverHandler : IRequestHandler<UpdateDocumentReceiver, Result>
{
    private readonly IDocumentReceiverRepository _repository;

    public UpdateDocumentReceiverHandler(IDocumentReceiverRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(UpdateDocumentReceiver request, CancellationToken cancellationToken)
    {
        var receiver = await _repository.GetByIdAsync(request.Id, cancellationToken);
        if (receiver == null)
            return Result.Fail("Receiver not found");

        receiver.Name = request.Name;
        receiver.Position = request.Position;
        receiver.FullTitle = request.FullTitle;
        receiver.IsDefault = request.IsDefault;

        await _repository.UpdateAsync(receiver, cancellationToken);

        return Result.Ok();
    }
}
