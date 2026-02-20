using TaskTracker.Models.DTOs.Employee;

namespace TaskTracker.Controllers.Interfaces;

public interface IEmployeeService
{
    Task<IEnumerable<EmployeeResponseDto>> GetAllAsync();
    Task<EmployeeResponseDto?> GetByIdAsync(Guid id);
    Task<EmployeeResponseDto> CreateAsync(CreateEmployeeDto dto);
    Task<EmployeeResponseDto?> UpdateAsync(Guid id, UpdateEmployeeDto dto);
    Task<bool> DeleteAsync(Guid id);
}