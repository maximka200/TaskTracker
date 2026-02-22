namespace TaskTracker.Models.DTOs.TaskGroup;

public class UpdateTaskGroupDto
{
    public string? Name { get; set; }
    public Guid? ProjectId { get; set; }
}