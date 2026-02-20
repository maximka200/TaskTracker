namespace TaskTracker.Models.DTOs.Project;

public class UpdateProjectDto
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public Guid? ProjectLeadId { get; set; }
    public Guid? ProjectManagerId { get; set; }
}