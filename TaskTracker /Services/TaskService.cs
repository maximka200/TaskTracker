using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class TaskService(AppDbContext db) : ITaskService
{
    public async Task<IEnumerable<TaskItem>> GetTasksAsync(Guid? userId, Guid? groupId, Guid? projectId)
    {
        var query = db.Tasks
            .AsNoTracking()
            .AsQueryable();
        
        if (userId.HasValue)
        {
            query = query.Where(t => t.Executors.Any(e => e.EmployeeId == userId.Value)
                                     || t.Observers.Any(o => o.EmployeeId == userId.Value));
        }
        
        if (groupId.HasValue)
        {
            query = query.Where(t => t.TaskGroupId == groupId.Value);
        }
        
        if (projectId.HasValue)
        {
            query = query.Where(t => t.ProjectId == projectId.Value);
        }

        return await query.ToListAsync();
    }


    public async Task<TaskItem?> GetByIdAsync(Guid id)
    {
        return await db.Tasks
            .AsNoTracking()
            .Include(t => t.Executors).ThenInclude(te => te.Employee)
            .Include(t => t.Observers).ThenInclude(to => to.Employee)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<TaskItem> CreateAsync(TaskItem task)
    {
        await db.Tasks
            .AddAsync(task);
        await db.SaveChangesAsync();
        return task;
    }

    public async Task<TaskItem?> UpdateAsync(Guid id, TaskItem task)
    {
        var existingTask = await db.Tasks
            .Include(t => t.Executors)
            .Include(t => t.Observers)
            .FirstOrDefaultAsync(t => t.Id == id);

        if (existingTask == null) return null;
        
        existingTask.Title = task.Title;
        existingTask.Description = task.Description;
        existingTask.Priority = task.Priority;
        existingTask.DueDate = task.DueDate;
        existingTask.ProjectId = task.ProjectId;
        existingTask.TaskGroupId = task.TaskGroupId;

        await db.SaveChangesAsync();
        return existingTask;
    }


    public async Task<bool> AddExecutorAsync(Guid taskId, Employee employee)
    {
        var task = await db.Tasks
            .Include(t => t.Executors).Include(taskItem => taskItem.Observers)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null) return false;
        
        if (task.Observers.Any(o => o.EmployeeId == employee.Id)) return false;

        if (task.Executors.Any(te => te.EmployeeId == employee.Id)) return true;
        task.Executors.Add(new TaskExecutor
        {
            TaskItemId = task.Id,
            TaskItem = task,
            EmployeeId = employee.Id,
            Employee = employee
        });
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> AddObserverAsync(Guid taskId, Employee employee)
    {
        var task = await db.Tasks
            .Include(t => t.Observers).Include(taskItem => taskItem.Executors)
            .FirstOrDefaultAsync(t => t.Id == taskId);

        if (task == null) return false;
        
        if (task.Executors.Any(e => e.EmployeeId == employee.Id)) return false;

        if (task.Observers.Any(to => to.EmployeeId == employee.Id)) return true;
        task.Observers.Add(new TaskObserver
        {
            TaskItemId = task.Id,
            TaskItem = task,
            EmployeeId = employee.Id,
            Employee = employee
        });
        await db.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangeStatusAsync(Guid taskId, TaskStatus newStatus)
    {
        var task = await db.Tasks.FirstOrDefaultAsync(t => t.Id == taskId);
        if (task == null) return false;
        
        var valid = task.Status switch
        {
            TaskStatus.Backlog => newStatus == TaskStatus.Current,
            TaskStatus.Current => newStatus == TaskStatus.OnReview,
            TaskStatus.OnReview => newStatus == TaskStatus.Completed,
            TaskStatus.Completed => newStatus == TaskStatus.Cancelled,
            _ => false
        };

        if (!valid) return false;

        task.Status = newStatus;
        await db.SaveChangesAsync();
        return true;
    }
}