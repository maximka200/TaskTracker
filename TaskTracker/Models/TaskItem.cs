namespace TaskTracker.Models;

public class TaskItem
{
    public Guid Id { get; set; }
    public string Title { get; set; } 
    public string Description { get; set; } 
    public Guid ProjectId { get; set; }
    public Project Project { get; set; }
    public Guid TaskGroupId { get; set; }
    public TaskGroup TaskGroup { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }

    public ICollection<TaskExecutor> Executors { get; set; } = new List<TaskExecutor>();
    public ICollection<TaskObserver> Observers { get; set; } = new List<TaskObserver>();
}
