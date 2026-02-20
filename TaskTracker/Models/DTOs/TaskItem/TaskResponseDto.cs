namespace TaskTracker.Models.DTOs.TaskItem;

public class TaskResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public Guid ProjectId { get; set; }
    public Guid TaskGroupId { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }
    public List<Guid> ExecutorIds { get; set; } = [];
    public List<Guid> ObserverIds { get; set; } = [];
}