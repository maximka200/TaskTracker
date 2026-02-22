using TaskTracker.Models.DTOs.TaskHistory;

namespace TaskTracker.Controllers.Interfaces;

public interface ITaskHistoryService
{
    Task<IEnumerable<TaskHistoryDto>> GetHistoryAsync(Guid? taskId);
}