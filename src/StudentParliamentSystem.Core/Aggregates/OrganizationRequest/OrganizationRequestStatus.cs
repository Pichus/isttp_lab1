namespace StudentParliamentSystem.Core.Aggregates.OrganizationRequest;

public class OrganizationRequestStatus
{
    public Guid Id { get; init; }
    public string Name { get; init; }
    
    public ICollection<OrganizationRequest> OrganizationRequests { get; set; }
}