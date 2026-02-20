using TaskTracker.Models.DTOs.TaskItem;

namespace TaskTracker.Models.DTOs.Project;

public class ProjectResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } 
    public string Description { get; set; }
    public Guid ProjectLeadId { get; set; }
    public Guid ProjectManagerId { get; set; }
}