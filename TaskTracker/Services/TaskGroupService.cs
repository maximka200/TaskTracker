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

    public async Task<TaskGroupResponseDto> GetByIdAsync(Guid id)
    {
        var group = await db.TaskGroups
            .AsNoTracking()
            .FirstOrDefaultAsync(g => g.Id == id);

        if (group == null)
            throw new KeyNotFoundException("Task group not found");

        return new TaskGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name
        };
    }

    public async Task<TaskGroupResponseDto> CreateAsync(CreateTaskGroupDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Group name is required");

        var exists = await db.TaskGroups
            .AnyAsync(g => g.Name == dto.Name);

        if (exists)
            throw new InvalidOperationException("Task group with this name already exists");

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

    public async Task<TaskGroupResponseDto> UpdateAsync(Guid id, UpdateTaskGroupDto dto)
    {
        var existing = await db.TaskGroups.FindAsync(id);

        if (existing == null)
            throw new KeyNotFoundException("Task group not found");

        if (!string.IsNullOrWhiteSpace(dto.Name))
        {
            var nameExists = await db.TaskGroups
                .AnyAsync(g => g.Name == dto.Name && g.Id != id);

            if (nameExists)
                throw new InvalidOperationException("Task group name already in use");

            existing.Name = dto.Name;
        }

        await db.SaveChangesAsync();

        return new TaskGroupResponseDto
        {
            Id = existing.Id,
            Name = existing.Name
        };
    }

    public async Task DeleteAsync(Guid id)
    {
        var existing = await db.TaskGroups.FindAsync(id);

        if (existing == null)
            throw new KeyNotFoundException("Task group not found");

        var hasActiveTasks = await db.Tasks
            .AnyAsync(t =>
                t.TaskGroupId == id &&
                t.Status != TaskStatus.Cancelled);

        if (hasActiveTasks)
            throw new InvalidOperationException(
                "Cannot delete group with active tasks");

        db.TaskGroups.Remove(existing);
        await db.SaveChangesAsync();
    }
}