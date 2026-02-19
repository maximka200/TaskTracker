using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Repository;

namespace TaskTracker.Services;

public class EmployeeService(AppDbContext context) : IEmployeeService
{
    public async Task<IEnumerable<Employee>> GetAllAsync()
    {
        return await context.Employees
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Employee?> GetByIdAsync(Guid id)
    {
        return await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);
    }

    public async Task<Employee> CreateAsync(Employee employee)
    {
        employee.Id = Guid.NewGuid();

        await context.Employees.AddAsync(employee);
        await context.SaveChangesAsync();

        return employee;
    }

    public async Task<Employee?> UpdateAsync(Guid id, Employee employee)
    {
        var existing = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return null;

        existing.FirstName = employee.FirstName;
        existing.LastName = employee.LastName;

        await context.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null) return false;

        context.Employees.Remove(existing);
        await context.SaveChangesAsync();

        return true;
    }
}
