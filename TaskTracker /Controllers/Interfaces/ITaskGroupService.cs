using TaskTracker.Models;

namespace TaskTracker.Controllers.Interfaces;

public interface ITaskGroupService
{
    Task<IEnumerable<TaskGroup>> GetAllAsync();
    Task<TaskGroup?> GetByIdAsync(Guid id);
    Task<TaskGroup> CreateAsync(TaskGroup group);
    Task<TaskGroup?> UpdateAsync(Guid id, TaskGroup group);
    Task<bool> DeleteAsync(Guid id);
}
