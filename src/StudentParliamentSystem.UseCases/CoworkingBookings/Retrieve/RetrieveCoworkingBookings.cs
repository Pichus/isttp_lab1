namespace StudentParliamentSystem.UseCases.CoworkingBookings.Retrieve;

public record RetrieveCoworkingBookings(int PageNumber = 1, int PageSize = 10, string? StatusFilter = null);
