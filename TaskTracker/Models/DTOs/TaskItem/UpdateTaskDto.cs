namespace TaskTracker.Models.DTOs.TaskItem;

public class UpdateTaskDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? TaskGroupId { get; set; }
    public TaskPriority? Priority { get; set; }
}