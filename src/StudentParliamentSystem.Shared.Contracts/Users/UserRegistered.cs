namespace StudentParliamentSystem.Shared.Contracts.Users;

public record UserRegistered(Guid UserId, string Email, string FirstName, string LastName);