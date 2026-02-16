namespace StudentParliamentSystem.Core.Aggregates.OrganizationRequest;

public class OrganizationRequest
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid EventId { get; set; }
    public Guid StatusId { get; set; }
    public DateTime CreatedAtUtc { get; set; }

    public OrganizationRequestStatus Status { get; set; }
    public User.User User { get; set; }
    public Event.Event Event { get; set; }
}