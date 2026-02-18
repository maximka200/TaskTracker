namespace TaskTracker.Models;

public class TaskObserver
{
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}