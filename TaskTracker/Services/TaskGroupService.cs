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
                Name = g.Name,
                ProjectId = g.ProjectId
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
            .AnyAsync(g => g.Name == dto.Name 
                           || g.ProjectId == dto.ProjectId);

        if (exists)
            throw new InvalidOperationException("Task group with this name already exists");

        var group = new TaskGroup
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            ProjectId = dto.ProjectId
        };

        await db.TaskGroups.AddAsync(group);
        await db.SaveChangesAsync();

        return new TaskGroupResponseDto
        {
            Id = group.Id,
            Name = group.Name,
            ProjectId = dto.ProjectId
        };
    }

    public async Task<TaskGroupResponseDto> UpdateAsync(Guid id, UpdateTaskGroupDto dto)
    {
        var existing = await db.TaskGroups.FindAsync(id);
        if (existing == null)
            throw new KeyNotFoundException("Task group not found");

        var projectExisting = await db.Projects.FindAsync(dto.ProjectId);
        if (projectExisting == null)
            throw new KeyNotFoundException("Project not found");
        
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
            Name = existing.Name,
            ProjectId = dto.ProjectId
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
    
    public async Task<TaskGroup?> GetFullGroupAsync(Guid id)
    {
        return await db.TaskGroups
            .AsNoTracking()
            .Include(g => g.Tasks) 
            .ThenInclude(t => t.Executors)
            .ThenInclude(te => te.Employee)
            .Include(g => g.Tasks)
            .ThenInclude(t => t.Observers) 
            .ThenInclude(to => to.Employee)
            .FirstOrDefaultAsync(g => g.Id == id);
    }
}