namespace TaskTracker.Models.DTOs.TaskHistory;

public class TaskHistoryDto
{
    public Guid TaskId { get; set; }

    public string Property { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime ChangedAt { get; set; }
}