using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskGroup;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/taskgroups")]
public class TaskGroupController(ITaskGroupService taskGroupService, IPdfService pdfService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskGroupResponseDto>>> GetAll()
        => Ok(await taskGroupService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskGroupResponseDto>> Get(Guid id)
        => Ok(await taskGroupService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<TaskGroupResponseDto>> Create(
        [FromBody] CreateTaskGroupDto dto)
    {
        var created = await taskGroupService.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskGroupResponseDto>> Update(
        Guid id,
        [FromBody] UpdateTaskGroupDto dto)
    {
        var updated = await taskGroupService.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await taskGroupService.DeleteAsync(id);
        return NoContent();
    }
    
    [HttpGet("{id:guid}/report")]
    public async Task<IActionResult> GenerateGroupReport(Guid id)
    {
        var group = await taskGroupService.GetFullGroupAsync(id);
        var pdf = pdfService.GenerateTaskGroupReport(group);

        return File(pdf, "application/pdf", $"group_{id}.pdf");
    }
}