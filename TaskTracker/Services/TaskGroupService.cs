using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Models.DTOs.TaskGroup;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class TaskGroupService(AppDbContext db) : ITaskGroupService
{
    public async Task<IEnumerable<TaskGroupResponseDto>> GetAllAsync()
    {
        return await db.TaskGroups
            .AsNoTracking()
            .Select(g => new TaskGroupResponseDto
            {
                Id = g.Id,
                Name = g.Name
            })
            .ToListAsync();
    }

    public async Task<TaskGroupResponseDto?> GetByIdAsync(Guid id)
    {
        var group = await db.TaskGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null) return null;

        return new TaskGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name
        };
    }

    public async Task<TaskGroupResponseDto> CreateAsync(CreateTaskGroupDto dto)
    {
        var group = new TaskGroup
        {
            Id = Guid.NewGuid(),
            Name = dto.Name
        };

        await db.TaskGroups.AddAsync(group);
        await db.SaveChangesAsync();

        return new TaskGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name
        };
    }

    public async Task<TaskGroupResponseDto?> UpdateAsync(Guid id, UpdateTaskGroupDto dto)
    {
        var existing = await db.TaskGroups.FindAsync(id);
        if (existing == null) return null;

        if (dto.Name != null)
            existing.Name = dto.Name;
        
        await db.SaveChangesAsync();

        return new TaskGroupResponseDto
        {
            Id = existing.Id,
            Name = existing.Name
        };
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existing = await db.TaskGroups.FindAsync(id);
        if (existing == null) return false;

        var tasks = await db.Tasks
            .Where(t => t.TaskGroupId == id)
            .ToListAsync();
        
        if (tasks.Count > 0 || tasks.Any(t => t.Status != TaskStatus.Cancelled))
            return false;

        db.TaskGroups.Remove(existing);
        await db.SaveChangesAsync();

        return true;
    }
}