using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Models.DTOs.TaskItem;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync(Guid? userId, Guid? groupId, Guid? projectId)
    {
        var query = db.Tasks.AsNoTracking().AsQueryable();

        if (userId.HasValue)
            query = query.Where(t => t.Executors.Any(e => e.EmployeeId == userId.Value)
                                   || t.Observers.Any(o => o.EmployeeId == userId.Value));

        if (groupId.HasValue)
            query = query.Where(t => t.TaskGroupId == groupId.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query.Select(t => new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            ProjectId = t.ProjectId,
            TaskGroupId = t.TaskGroupId,
            Status = t.Status,
            Priority = t.Priority,
            ExecutorIds = t.Executors.Select(e => e.EmployeeId).ToList(),
            ObserverIds = t.Observers.Select(o => o.EmployeeId).ToList()
        }).ToListAsync();
    }

    public async Task<TaskResponseDto?> GetByIdAsync(Guid id)
    {
        var t = await db.Tasks
            .AsNoTracking()
            .Include(t => t.Executors)
            .Include(t => t.Observers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (t == null) return null;

        return new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            ProjectId = t.ProjectId,
            TaskGroupId = t.TaskGroupId,
            Status = t.Status,
            Priority = t.Priority,
            ExecutorIds = t.Executors.Select(e => e.EmployeeId).ToList(),
            ObserverIds = t.Observers.Select(o => o.EmployeeId).ToList()
        };
    }

    public async Task<TaskResponseDto> CreateAsync(CreateTaskDto dto)
    {
        var t = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            TaskGroupId = dto.TaskGroupId,
            Status = dto.Status,
            Priority = dto.Priority
        };

        await db.Tasks.AddAsync(t);
        await db.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            ProjectId = t.ProjectId,
            TaskGroupId = t.TaskGroupId,
            Status = t.Status,
            Priority = t.Priority,
            ExecutorIds = [],
            ObserverIds = []
        };
    }

    public async Task<TaskResponseDto?> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var t = await db.Tasks.Include(t => t.Executors).Include(t => t.Observers).FirstOrDefaultAsync(t => t.Id == id);
        if (t == null) return null;
        
        if (dto.Title != null) 
            t.Title = dto.Title;
        if (dto.Description != null) 
            t.Description = dto.Description;
        if (dto.ProjectId != null) 
            t.ProjectId = dto.ProjectId.Value;
        if (dto.TaskGroupId != null) 
            t.TaskGroupId = dto.TaskGroupId.Value;
        if (dto.Priority != null) 
            t.Priority = dto.Priority.Value;

        await db.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = t.Id,
            Title = t.Title,
            Description = t.Description,
            ProjectId = t.ProjectId,
            TaskGroupId = t.TaskGroupId,
            Status = t.Status,
            Priority = t.Priority,
            ExecutorIds = t.Executors.Select(e => e.EmployeeId).ToList(),
            ObserverIds = t.Observers.Select(o => o.EmployeeId).ToList()
        };
    }

    public async Task<bool> AddExecutorAsync(Guid taskId, Guid employeeId)
    {
        var task = await db.Tasks.Include(t => t.Executors).Include(t => t.Observers).FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null) return false;

        if (task.Observers.Any(o => o.EmployeeId == employeeId)) return false;
        if (task.Executors.Any(e => e.EmployeeId == employeeId)) return true;

        task.Executors.Add(new TaskExecutor { TaskItemId = taskId, EmployeeId = employeeId });
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> AddObserverAsync(Guid taskId, Guid employeeId)
    {
        var task = await db.Tasks.Include(t => t.Executors).Include(t => t.Observers).FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null) return false;

        if (task.Executors.Any(e => e.EmployeeId == employeeId)) return false;
        if (task.Observers.Any(o => o.EmployeeId == employeeId)) return true;

        task.Observers.Add(new TaskObserver { TaskItemId = taskId, EmployeeId = employeeId });
        await db.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid taskId, TaskStatus newStatus)
    {
        var t = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (t == null) return false;

        var valid = t.Status switch
        {
            TaskStatus.Backlog => newStatus == TaskStatus.Current,
            TaskStatus.Current => newStatus == TaskStatus.OnReview,
            TaskStatus.OnReview => newStatus == TaskStatus.Completed,
            TaskStatus.Completed => newStatus == TaskStatus.Cancelled,
            _ => false
        };

        if (!valid) return false;

        t.Status = newStatus;
        await db.SaveChangesAsync();
        return true;
    }
}