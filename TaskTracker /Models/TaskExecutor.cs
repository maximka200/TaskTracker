namespace TaskTracker.Models;

public class TaskExecutor
{
    public int TaskItemId { get; set; }
    public TaskItem TaskItem { get; set; }

    public int EmployeeId { get; set; }
    public Employee Employee { get; set; }
}