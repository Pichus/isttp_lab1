namespace StudentParliamentSystem.Core.Aggregates.Event;

public class Event
{
    public Guid Id { get; init; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string Location { get; set; }
    public int? MaxParticipants { get; set; }
    public bool IsPublished { get; set; }
    public DateTime StartTimeUtc { get; set; }
    public DateTime EndTimeUtc { get; set; }
    public Guid DepartmentId { get; set; }
    public Guid CreatedByUserId { get; init; }
    public DateTime CreatedAtUtc { get; init; }

    public User.User CreatedByUser { get; init; }
    public Department.Department Department { get; set; }
    public ICollection<EventOrganizer> EventOrganizers { get; set; }
    public ICollection<EventTag> Tags { get; set; }
    public ICollection<EventRegistration> Registrations { get; set; }
    public ICollection<OrganizationRequest.OrganizationRequest> OrganizationRequests { get; set; }
}