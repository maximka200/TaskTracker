namespace TaskTracker.Models;

public class Project
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;
    public string Description { get; set; } = default!;

    public Guid ProjectLeadId { get; set; }
    public Employee ProjectLead { get; set; } = default!;

    public Guid ProjectManagerId { get; set; }
    public Employee ProjectManager { get; set; } = default!;

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
