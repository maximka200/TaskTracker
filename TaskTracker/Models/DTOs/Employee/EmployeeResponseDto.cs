namespace TaskTracker.Models.DTOs.Employee;

public class EmployeeResponseDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; } 
    public string MiddleName { get; set; }
    public string UserName { get; set; }
    public EmployeeRole Role { get; set; }
}