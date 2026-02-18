namespace TaskTracker.Models;

public class Project(string name, string description, Employee projectLead, Employee projectManager)
{
    public int Id { get; set; }

    public string Name { get; set; } = name;
    public string Description { get; set; } = description;

    public int ProjectLeadId { get; set; } = projectLead.Id;
    public Employee ProjectLead { get; set; } = projectLead;

    public int ProjectManagerId { get; set; } = projectManager.Id;
    public Employee ProjectManager { get; set; } = projectManager;
}
