namespace StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

public class DocumentReceiver
{
    public Guid Id { get; init; }
    public string Name { get; set; } = string.Empty;
    public string Position { get; set; } = string.Empty;
    public string FullTitle { get; set; } = string.Empty;
    public bool IsDefault { get; set; }
}
