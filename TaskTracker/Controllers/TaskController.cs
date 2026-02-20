using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskItem;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? userId, [FromQuery] Guid? groupId, [FromQuery] Guid? projectId)
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
                Status = 404
            });

        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        if (dto.Status != TaskStatus.Backlog && dto.Status != TaskStatus.Current)
        {
            return BadRequest(new ProblemDetails
            {
                Title = "Invalid status",
                Detail = "Task can only be created in Backlog or Current status",
                Status = 400
            });
        }

        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var existing = await service.GetByIdAsync(id);
        if (existing is null)
            return NotFound(new ProblemDetails { Title = "Task not found", Status = 404 });

        var updated = await service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/executors/{employeeId:guid}")]
    public async Task<IActionResult> AddExecutor(Guid id, Guid employeeId)
    {
        var result = await service.AddExecutorAsync(id, employeeId);
        if (!result) return BadRequest(new ProblemDetails { Title = "Cannot add executor", Status = 400 });

        return Ok();
    }

    [HttpPost("{id:guid}/observers/{employeeId:guid}")]
    public async Task<IActionResult> AddObserver(Guid id, Guid employeeId)
    {
        var result = await service.AddObserverAsync(id, employeeId);
        if (!result) return BadRequest(new ProblemDetails { Title = "Cannot add observer", Status = 400 });

        return Ok();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] TaskStatus status)
    {
        var result = await service.ChangeStatusAsync(id, status);
        if (!result)
            return BadRequest(new ProblemDetails { Title = "Invalid status transition", Status = 400 });

        return Ok();
    }
}
