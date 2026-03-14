namespace StudentParliamentSystem.UseCases.Users.Retrieve.All;

public record RetrieveAllUsers(int PageNumber, int PageSize, string? Query);