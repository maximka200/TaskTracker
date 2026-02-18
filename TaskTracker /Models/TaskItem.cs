namespace TaskTracker.Models;

public class TaskItem
{
    public int Id { get; set; }
    public string Title { get; set; } 
    public string Description { get; set; } 
    public int ProjectId { get; set; }
    public Project Project { get; set; }
    public int TaskGroupId { get; set; }
    public TaskGroup TaskGroup { get; set; }
    public DateTime DueDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public TaskStatus Status { get; set; }
    public TaskPriority Priority { get; set; }

    public ICollection<TaskExecutor> Executors { get; set; } = new List<TaskExecutor>();
    public ICollection<TaskObserver> Observers { get; set; } = new List<TaskObserver>();

    
    public void ChangeStatus(TaskStatus status)
    {
        Status = status;
    }

    public void AddExecutor(Employee employee)
    {
        throw new NotImplementedException();
    }

    public void AddObserver(Employee employee)
    {
        throw new NotImplementedException();
    }
}
