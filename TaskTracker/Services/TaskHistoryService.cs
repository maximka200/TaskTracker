using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskHistory;
using TaskTracker.Repository;

namespace TaskTracker.Services;

public class TaskHistoryService(AppDbContext db) : ITaskHistoryService
{
    public async Task<IEnumerable<TaskHistoryDto>> GetHistoryAsync(Guid? taskId)
    {
        var query = db.TaskHistories
            .AsNoTracking()
            .OrderByDescending(h => h.ChangedAt)
            .AsQueryable();

        if (taskId.HasValue)
            query = query.Where(h => h.TaskId == taskId.Value);

        return await query
            .Select(h => new TaskHistoryDto
            {
                TaskId = h.TaskId,
                Property = h.PropertyName,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangedAt = h.ChangedAt
            })
            .ToListAsync();
    }
}