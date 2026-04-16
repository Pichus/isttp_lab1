namespace StudentParliamentSystem.Shared.Contracts.Users;

public record UserDetailsUpdated(Guid UserId, string FirstName, string LastName, ICollection<string> Roles);