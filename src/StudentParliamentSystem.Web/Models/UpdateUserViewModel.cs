using System.ComponentModel.DataAnnotations;

namespace StudentParliamentSystem.Api.Models;

public class UpdateUserViewModel
{
    public Guid Id { get; set; }

    [Required]
    public string FirstName { get; set; } = null!;

    [Required]
    public string LastName { get; set; } = null!;

    [Required]
    public List<string> Roles { get; set; } = new();

    public List<string> AvailableRoles { get; set; } = Enum.GetNames<StudentParliamentSystem.Core.Aggregates.Role.RoleName>().ToList();
}
