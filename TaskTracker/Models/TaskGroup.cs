namespace TaskTracker.Models;

public class TaskGroup
{
    public Guid Id { get; set; }

    public string Name { get; set; }
    
    public Guid? ProjectId { get; set; }
    public Project Project { get; set; }
    
    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}