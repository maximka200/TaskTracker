using TaskTracker.Models.DTOs;
using TaskTracker.Models.DTOs.TaskGroup;

namespace TaskTracker.Controllers.Interfaces;

public interface ITaskGroupService
{
    Task<IEnumerable<TaskGroupResponseDto>> GetAllAsync();
    Task<TaskGroupResponseDto> GetByIdAsync(Guid id);
    Task<TaskGroupResponseDto> CreateAsync(CreateTaskGroupDto dto);
    Task<TaskGroupResponseDto> UpdateAsync(Guid id, UpdateTaskGroupDto dto);
    Task DeleteAsync(Guid id);
}
