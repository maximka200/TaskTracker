namespace TaskTracker.Models.DTOs.Employee;

public class CreateEmployeeDto
{
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string MiddleName { get; set; } 
    public string UserName { get; set; } 
    public EmployeeRole Role { get; set; }
}