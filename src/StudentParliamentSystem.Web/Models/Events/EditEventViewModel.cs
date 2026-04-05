using System.ComponentModel.DataAnnotations;

namespace StudentParliamentSystem.Api.Models.Events;

public class EditEventViewModel
{
    [Required]
    public Guid Id { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; }

    [Required]
    public string Description { get; set; }

    [Required]
    [StringLength(200)]
    public string Location { get; set; }

    [Required]
    public DateTime StartTime { get; set; }

    [Required]
    public DateTime EndTime { get; set; }

    [Required]
    public Guid DepartmentId { get; set; }

    public int? MaxParticipants { get; set; }

    public bool IsPublished { get; set; }

    // Using CSV for the tags selected by Tagify / Badge custom input
    public string TagsCsv { get; set; }
}
