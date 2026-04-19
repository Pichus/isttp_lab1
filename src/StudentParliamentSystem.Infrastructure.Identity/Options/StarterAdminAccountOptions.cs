using System.ComponentModel.DataAnnotations;

namespace StudentParliamentSystem.Infrastructure.Identity.Options;

public class StarterAdminAccountOptions
{
    public const string SectionName = "StarterAdminAccount";

    [Required(ErrorMessage =
        "The starter admin account Name is missing." +
        "Please ensure 'StarterAdminAccount:Name' is configured in your settings.")]
    public string Name { get; init; } = string.Empty;

    [Required(ErrorMessage =
        "The starter admin account Email is missing." +
        "Please ensure 'StarterAdminAccount:Email' is configured in your settings.")]
    public string Email { get; init; } = string.Empty;

    [Required(ErrorMessage =
        "The starter admin account Password is missing." +
        "Please ensure 'StarterAdminAccount:Password' is configured in your settings.")]
    public string Password { get; init; } = string.Empty;
}