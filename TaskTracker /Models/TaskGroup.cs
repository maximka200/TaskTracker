namespace TaskTracker.Models;

public class TaskGroup(string name)
{
    public int Id { get; set; }

    public string Name { get; set; } = name;
}