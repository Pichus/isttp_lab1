using FluentResults;
using StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

namespace StudentParliamentSystem.UseCases.CoworkingBookings.DocumentReceivers.Create;

public record CreateDocumentReceiverCommand(string Name, string Position, string FullTitle, bool IsDefault);

public class CreateDocumentReceiverCommandHandler
{
    private readonly IDocumentReceiverRepository _repository;

    public CreateDocumentReceiverCommandHandler(IDocumentReceiverRepository repository)
    {
        _repository = repository;
    }

    public async Task<Result> Handle(CreateDocumentReceiverCommand command, CancellationToken cancellationToken)
    {
        var receiver = new DocumentReceiver
        {
            Id = Guid.NewGuid(),
            Name = command.Name,
            Position = command.Position,
            FullTitle = command.FullTitle,
            IsDefault = command.IsDefault
        };

        await _repository.AddAsync(receiver, cancellationToken);
        return Result.Ok();
    }
}
