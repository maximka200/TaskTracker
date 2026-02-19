using TaskTracker.Models;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Controllers.Interfaces;

public interface ITaskService
{
    Task<IEnumerable<TaskItem>> GetTasksAsync(Guid? userId, Guid? groupId, Guid? projectId);

    Task<TaskItem?> GetByIdAsync(Guid id);

    Task<TaskItem> CreateAsync(TaskItem task);

    Task<TaskItem?> UpdateAsync(Guid id, TaskItem updatedTask);

    Task<bool> AddExecutorAsync(Guid taskId, Employee employee);

    Task<bool> AddObserverAsync(Guid taskId, Employee employee);

    Task<bool> ChangeStatusAsync(Guid taskId, TaskStatus newStatus);
}
