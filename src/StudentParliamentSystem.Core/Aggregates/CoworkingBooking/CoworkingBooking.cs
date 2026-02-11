namespace StudentParliamentSystem.Core.Aggregates.CoworkingBooking;

public class CoworkingBooking
{
    public Guid Id { get; init; }
    public Guid EventId { get; init; }
    public Guid StatusId { get; set; }
    public Guid? SpaceManagerId { get; set; }
    public string? Notes { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }

    public CoworkingBookingStatus Status { get; set; }
    public Event.Event Event { get; init; }
    public User.User? SpaceManager { get; set; }
}