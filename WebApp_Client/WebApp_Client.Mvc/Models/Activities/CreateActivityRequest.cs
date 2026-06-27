using System.ComponentModel.DataAnnotations;

namespace WebApp_Client.Mvc.Models.Activities;

public class CreateActivityRequest
{
    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    public DateTime? StartTime { get; set; }

    [Required]
    public DateTime? EndTime { get; set; }

    [Range(1, 1000)]
    public int MaxParticipants { get; set; }

    [Range(1, 5, ErrorMessage = "Type must be selected")]
    public int Type { get; set; }
}
