using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskItem;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/tasks")]
public class TaskController(ITaskService taskService, IPdfService pdfService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] Guid? userId, [FromQuery] Guid? groupId,
        [FromQuery] Guid? projectId)
    {
        var tasks = await taskService.GetTasksAsync(userId, groupId, projectId);
        return Ok(tasks);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var task = await taskService.GetByIdAsync(id);
        return Ok(task);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskDto dto)
    {
        var created = await taskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskDto dto)
    {
        var updated = await taskService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpPost("{id:guid}/executors/{employeeId:guid}")]
    public async Task<IActionResult> AddExecutor(Guid id, Guid employeeId)
    {
        await taskService.AddExecutorAsync(id, employeeId);
        return NoContent();
    }

    [HttpPost("{id:guid}/observers/{employeeId:guid}")]
    public async Task<IActionResult> AddObserver(Guid id, Guid employeeId)
    {
        await taskService.AddObserverAsync(id, employeeId);
        return NoContent();
    }

    [HttpPatch("{id:guid}/status")]
    public async Task<IActionResult> ChangeStatus(Guid id, [FromQuery] TaskStatus status)
    {
        await taskService.ChangeStatusAsync(id, status);
        return NoContent();
    }
    
    [HttpGet("{id:guid}/report")]
    public async Task<IActionResult> GenerateReport(Guid id)
    {
        var task = await taskService.GetFullTaskAsync(id);
        var pdfBytes = pdfService.GenerateTaskReport(task);

        return File(pdfBytes, "application/pdf", $"task_{id}.pdf");
    }
}