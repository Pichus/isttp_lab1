using Microsoft.AspNetCore.Identity;

namespace StudentParliamentSystem.Infrastructure.Identity.Data.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}