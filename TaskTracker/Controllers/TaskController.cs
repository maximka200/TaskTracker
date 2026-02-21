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
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/executors/{employeeId:guid}")]
    public async Task<IActionResult> AddExecutor(Guid id, Guid employeeId)
    {
        await service.AddExecutorAsync(id, employeeId);
        return NoContent();
    }

    [HttpPost("{id:guid}/observers/{employeeId:guid}")]
    public async Task<IActionResult> AddObserver(Guid id, Guid employeeId)
    {
        await service.AddObserverAsync(id, employeeId);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] TaskStatus status)
    {
        await service.ChangeStatusAsync(id, status);
        return NoContent();
    }
}