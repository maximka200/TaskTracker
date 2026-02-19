using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class TaskGroupService(AppDbContext db) : ITaskGroupService
{
    public async Task<IEnumerable<TaskGroup>> GetAllAsync()
    {
        return await db.TaskGroups
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<TaskGroup?> GetByIdAsync(Guid id)
    {
        return await db.TaskGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<TaskGroup> CreateAsync(TaskGroup group)
    {
        await db.TaskGroups.AddAsync(group);  
        await db.SaveChangesAsync();         
        return group; 
    }

    public async Task<TaskGroup?> UpdateAsync(Guid id, TaskGroup group)
    {
        var existingTaskGroup = await db.TaskGroups.FindAsync(id);
        if (existingTaskGroup == null) 
            return null;

        existingTaskGroup.Name = group.Name;
        await db.SaveChangesAsync();

        return existingTaskGroup;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existingTaskGroup = await db.TaskGroups.FindAsync(id);
        if (existingTaskGroup == null) 
            return false;

        var tasks = await db.Tasks
            .Where(t => t.TaskGroupId == id)
            .ToListAsync();

        if (tasks.Count > 0 || tasks.Any(t => t.Status != TaskStatus.Cancelled))
            return false;

        db.TaskGroups.Remove(existingTaskGroup);
        await db.SaveChangesAsync();
        return true;
    }
}