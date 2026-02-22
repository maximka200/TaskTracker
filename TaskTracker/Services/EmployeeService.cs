using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Models.DTOs.Employee;
using TaskTracker.Repository;

namespace TaskTracker.Services;

public class EmployeeService(AppDbContext context) : IEmployeeService
{
    public async Task<IEnumerable<EmployeeResponseDto>> GetAllAsync()
    {
        return await context.Employees
            .AsNoTracking()
            .Select(e => new EmployeeResponseDto
            {
                Id = e.Id,
                FirstName = e.FirstName,
                LastName = e.LastName,
                MiddleName = e.MiddleName,
                UserName = e.UserName,
                Role = e.Role
            })
            .ToListAsync();
    }

    public async Task<EmployeeResponseDto> GetByIdAsync(Guid id)
    {
        var e = await context.Employees
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id);

        if (e == null)
            throw new KeyNotFoundException("Employee not found");

        return new EmployeeResponseDto
        {
            Id = e.Id,
            FirstName = e.FirstName,
            LastName = e.LastName,
            MiddleName = e.MiddleName,
            UserName = e.UserName,
            Role = e.Role
        };
    }

    public async Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto)
    {
        var employee = new Employee(
            dto.FirstName,
            dto.LastName,
            dto.MiddleName,
            dto.UserName,
            dto.Role
        );

        await context.Employees.AddAsync(employee);
        await context.SaveChangesAsync();

        return new EmployeeResponseDto
        {
            Id = employee.Id,
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            MiddleName = employee.MiddleName,
            UserName = employee.UserName,
            Role = employee.Role
        };
    }

    public async Task<EmployeeResponseDto> UpdateAsync(Guid id, UpdateEmployeeDto dto)
    {
        var existing = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null)
            throw new KeyNotFoundException("Employee not found");

        if (dto.FirstName != null)
            existing.FirstName = dto.FirstName;
        if (dto.LastName != null) 
            existing.LastName = dto.LastName;
        if (dto.MiddleName != null) 
            existing.MiddleName = dto.MiddleName;
        if (dto.UserName != null) 
            existing.UserName = dto.UserName;
        if (dto.Role != null) 
            existing.Role = dto.Role.Value;

        await context.SaveChangesAsync();

        return new EmployeeResponseDto
        {
            Id = existing.Id,
            FirstName = existing.FirstName,
            LastName = existing.LastName,
            MiddleName = existing.MiddleName,
            UserName = existing.UserName,
            Role = existing.Role
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await context.Employees.FirstOrDefaultAsync(e => e.Id == id);
        if (existing == null)
            throw new KeyNotFoundException("Employee not found");

        context.Employees.Remove(existing);
        await context.SaveChangesAsync();
    }
}