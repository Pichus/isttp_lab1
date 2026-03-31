using System.ComponentModel.DataAnnotations;

using StudentParliamentSystem.Core.Aggregates.Role;

namespace StudentParliamentSystem.Seeding.Options;

public class InitialAdminAccountOptions
{
    public const string SectionName = "InitialAdminAccount";

    [Required(ErrorMessage =
        "The starter admin account First name is missing." +
        "Please ensure 'StarterAdminAccount:FirstName' is configured in your settings.")]
    public string FirstName { get; init; } = string.Empty;

    [Required(ErrorMessage =
        "The starter admin account Last name is missing." +
        "Please ensure 'StarterAdminAccount:LastName' is configured in your settings.")]
    public string LastName { get; init; } = string.Empty;

    [Required(ErrorMessage =
        "The starter admin account Email is missing." +
        "Please ensure 'StarterAdminAccount:Email' is configured in your settings.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage =
        "The starter admin account Password is missing." +
        "Please ensure 'StarterAdminAccount:Password' is configured in your settings.")]
    public string Password { get; init; } = string.Empty;

    [Required(ErrorMessage =
        "The starter admin account Roles are missing." +
        "Please ensure 'StarterAdminAccount:Roles' is configured in your settings.")]
    public IEnumerable<RoleName> Roles { get; init; } = [];
}