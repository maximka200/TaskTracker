namespace TaskTracker.Models.DTOs.TaskGroup;

public class CreateTaskGroupDto
{
    public string Name { get; set; } 
    public Guid ProjectId { get; set; } 
}