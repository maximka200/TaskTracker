namespace TaskTracker.Models;

public class TaskHistory
{
    public Guid Id { get; set; }

    public Guid TaskId { get; set; }
    public TaskItem Task { get; set; }

    public string PropertyName { get; set; } = null!;

    public string? OldValue { get; set; }
    public string? NewValue { get; set; }

    public DateTime ChangedAt { get; set; }

    public Guid? ChangedByEmployeeId { get; set; }
}