using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Models.DTOs.TaskItem;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    private static readonly Dictionary<TaskStatus, TaskStatus[]> ValidTransitions = new()
    {
        [TaskStatus.Backlog] = [TaskStatus.Current, TaskStatus.Cancelled],
        [TaskStatus.Current] = [TaskStatus.Active, TaskStatus.Cancelled],
        [TaskStatus.Active] = [TaskStatus.OnReview, TaskStatus.Cancelled],
        [TaskStatus.OnReview] = [TaskStatus.Completed, TaskStatus.Cancelled, TaskStatus.Active],
        [TaskStatus.Completed] = [],
        [TaskStatus.Cancelled] = []
    };
    
    public async Task<IEnumerable<TaskResponseDto>> GetTasksAsync(
        Guid? userId,
        Guid? groupId,
        Guid? projectId)
    {
        var query = db.Tasks.AsNoTracking().AsQueryable();

        if (userId.HasValue)
        {
            query = query.Where(t =>
                t.Executors.Any(e => e.EmployeeId == userId.Value) ||
                t.Observers.Any(o => o.EmployeeId == userId.Value));
        }

        if (groupId.HasValue)
            query = query.Where(t => t.TaskGroupId == groupId.Value);

        if (projectId.HasValue)
            query = query.Where(t => t.ProjectId == projectId.Value);

        return await query
            .Select(t => new TaskResponseDto
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
            })
            .ToListAsync();
    }

    public async Task<TaskResponseDto> GetByIdAsync(Guid id)
    {
        var t = await db.Tasks
            .AsNoTracking()
            .Include(t => t.Executors)
            .Include(t => t.Observers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (t == null)
            throw new KeyNotFoundException("Task not found");

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
        if (string.IsNullOrWhiteSpace(dto.Title))
            throw new ArgumentException("Title is required");

        var projectExisting = await db.Projects
            .AnyAsync(project => project.Id == dto.ProjectId);
        if (!projectExisting)
            throw new KeyNotFoundException("Project not found");
        
        var groupExisting = await db.Projects
            .AnyAsync(project => project.Id == dto.ProjectId);
        if (!groupExisting)
            throw new KeyNotFoundException("Task group not found");

        var task = new TaskItem
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            ProjectId = dto.ProjectId,
            TaskGroupId = dto.TaskGroupId,
            Status = dto.Status,
            Priority = dto.Priority
        };

        await db.Tasks.AddAsync(task);
        await db.SaveChangesAsync();

        return new TaskResponseDto
        {
            Id = task.Id,
            Title = task.Title,
            Description = task.Description,
            ProjectId = task.ProjectId,
            TaskGroupId = task.TaskGroupId,
            Status = task.Status,
            Priority = task.Priority,
            ExecutorIds = [],
            ObserverIds = []
        };
    }

    public async Task<TaskResponseDto> UpdateAsync(Guid id, UpdateTaskDto dto)
    {
        var t = await db.Tasks
            .Include(t => t.Executors)
            .Include(t => t.Observers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (t == null)
            throw new KeyNotFoundException("Task not found");

        if (dto.Title != null)
            t.Title = dto.Title;

        if (dto.Description != null)
            t.Description = dto.Description;

        if (dto.ProjectId.HasValue)
            t.ProjectId = dto.ProjectId.Value;

        if (dto.TaskGroupId.HasValue)
            t.TaskGroupId = dto.TaskGroupId.Value;

        if (dto.Priority.HasValue)
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

    public async Task AddExecutorAsync(Guid taskId, Guid employeeId)
    {
        var task = await db.Tasks
            .Include(t => t.Executors)
            .Include(t => t.Observers)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new KeyNotFoundException("Task not found");

        if (task.Observers.Any(o => o.EmployeeId == employeeId))
            throw new InvalidOperationException("Employee already assigned as observer");

        if (task.Executors.Any(e => e.EmployeeId == employeeId))
            throw new InvalidOperationException("Employee already assigned as executor");

        task.Executors.Add(new TaskExecutor
        {
            TaskItemId = taskId,
            EmployeeId = employeeId
        });

        await db.SaveChangesAsync();
    }

    public async Task AddObserverAsync(Guid taskId, Guid employeeId)
    {
        var task = await db.Tasks
            .Include(t => t.Executors)
            .Include(t => t.Observers)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null)
            throw new KeyNotFoundException("Task not found");

        if (task.Executors.Any(e => e.EmployeeId == employeeId))
            throw new InvalidOperationException("Employee already assigned as executor");

        if (task.Observers.Any(o => o.EmployeeId == employeeId))
            throw new InvalidOperationException("Employee already assigned as observer");

        task.Observers.Add(new TaskObserver
        {
            TaskItemId = taskId,
            EmployeeId = employeeId
        });

        await db.SaveChangesAsync();
    }

    public async Task ChangeStatusAsync(Guid taskId, TaskStatus newStatus)
    {
        var t = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);

        if (t == null)
            throw new KeyNotFoundException("Task not found");

        var valid = ValidTransitions.TryGetValue(t.Status, out var allowed)
                    && allowed.Contains(newStatus);

        if (!valid)
            throw new InvalidOperationException(
                $"Invalid transition from {t.Status} to {newStatus}");

        t.Status = newStatus;
        await db.SaveChangesAsync();
    }
    
    public async Task<TaskItem?> GetFullTaskAsync(Guid id)
    {
        return await db.Tasks
            .Include(t => t.Project)
            .Include(t => t.TaskGroup)
            .Include(t => t.Executors)
            .ThenInclude(e => e.Employee)
            .Include(t => t.Observers)
            .ThenInclude(o => o.Employee)
            .FirstOrDefaultAsync(t => t.Id == id);
    }
}