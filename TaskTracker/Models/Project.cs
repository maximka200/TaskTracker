namespace TaskTracker.Models;

public class Project
{
    public Guid Id { get; set; }

    public string Name { get; set; } 
    public string Description { get; set; } 

    public Guid ProjectLeadId { get; set; }
    public Employee ProjectLead { get; set; } 

    public Guid ProjectManagerId { get; set; }
    public Employee ProjectManager { get; set; }

    public ICollection<TaskItem> Tasks { get; set; } = new List<TaskItem>();
}
