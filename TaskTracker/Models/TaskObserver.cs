namespace TaskTracker.Models;

public class TaskObserver
{
    public Guid TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; }

    public Guid EmployeeId { get; set; }
    public Employee Employee { get; set; }
}