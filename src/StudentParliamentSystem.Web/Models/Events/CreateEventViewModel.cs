using System.ComponentModel.DataAnnotations;

namespace StudentParliamentSystem.Api.Models.Events;

public class CreateEventViewModel
{
    [Required]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    public string Location { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }

    public int? MaxParticipants { get; set; }

    public bool IsPublished { get; set; }

    public string? TagsCsv { get; set; }
}