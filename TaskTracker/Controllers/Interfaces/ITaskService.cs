using TaskTracker.Models.DTOs.Employee;
using TaskTracker.Models.DTOs.TaskItem;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Controllers.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskResponseDto>> GetTasksAsync(Guid? userId = null, Guid? groupId = null, Guid? projectId = null);
    Task<TaskResponseDto?> GetByIdAsync(Guid id);
    Task<TaskResponseDto> CreateAsync(CreateTaskDto dto);
    Task<TaskResponseDto?> UpdateAsync(Guid id, UpdateTaskDto dto);
    Task<bool> AddExecutorAsync(Guid taskId, Guid employeeId);
    Task<bool> AddObserverAsync(Guid taskId, Guid employeeId);
    Task<bool> ChangeStatusAsync(Guid taskId, TaskStatus newStatus);
}