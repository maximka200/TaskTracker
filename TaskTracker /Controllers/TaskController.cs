using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService service, IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromQuery] Guid? userId,
        [FromQuery] Guid? groupId,
        [FromQuery] Guid? projectId)
    {
        var tasks = await service.GetTasksAsync(userId, groupId, projectId);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await service.GetByIdAsync(id);
        if (task is null)
            return NotFound(new ProblemDetails
            {
                Title = "Task not found",
                Status = StatusCodes.Status404NotFound
            });

        return Ok(task);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create(TaskItem task)
    {
        if (task.Status != TaskStatus.Backlog &&
            task.Status != TaskStatus.Current)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid status",
                Detail = "Task can only be created in Backlog or Current status",
                Status = StatusCodes.Status400BadRequest
            });
        }

        var created = await service.CreateAsync(task);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, TaskItem task)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ProblemDetails { Title = "Task not found", Status = 404 });

        if (task.Status != existing.Status)
            return BadRequest(new ProblemDetails
            {
                Title = "Status change not allowed here",
                Status = StatusCodes.Status400BadRequest
            });

        var updated = await service.UpdateAsync(id, task);
        return Ok(updated);
    }
    
    [HttpPost("{id:guid}/executors/{employeeId:guid}")]
    public async Task<IActionResult> AddExecutor(Guid id, Guid employeeId)
    {
        var employee = await employeeService.GetByIdAsync(employeeId);
        if (employee is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Employee not found or id not correct",
                Status = StatusCodes.Status404NotFound
            });
        }
        
        var result = await service.AddExecutorAsync(id, employee);
        if (!result)
            return BadRequest(new ProblemDetails
            {
                Title = "Cannot add executor",
                Status = StatusCodes.Status400BadRequest
            });

        return Ok();
    }
    
    [HttpPost("{id:guid}/observers/{employeeId:guid}")]
    public async Task<IActionResult> AddObserver(Guid id, Guid employeeId)
    {
        var employee = await employeeService.GetByIdAsync(employeeId);
        if (employee is null)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Employee not found or id not correct",
                Status = StatusCodes.Status404NotFound
            });
        }
        
        var result = await service.AddExecutorAsync(id, employee);
        if (!result)
            return BadRequest(new ProblemDetails
            {
                Title = "Cannot add observer",
                Status = StatusCodes.Status400BadRequest
            });

        return Ok();
    }
    
    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] TaskStatus status)
    {
        var result = await service.ChangeStatusAsync(id, status);
        if (!result)
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid status transition",
                Status = StatusCodes.Status400BadRequest
            });

        return Ok();
    }
}
