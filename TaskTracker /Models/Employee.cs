namespace TaskTracker.Models;

public class Employee(string lastName, string firstName, string middleName, string userName, EmployeeRole role)
{
    public int Id { get; set; }

    public string LastName { get; set; } = lastName;
    public string FirstName { get;  set; } = firstName;
    public string MiddleName { get; set; } = middleName;

    public string UserName { get; set; } = userName;
    public EmployeeRole Role { get; set; } = role;
}