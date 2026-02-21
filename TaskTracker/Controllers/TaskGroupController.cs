using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskGroup;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/taskgroups")]
public class TaskGroupController(ITaskGroupService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskGroupResponseDto>>> GetAll()
        => Ok(await service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TaskGroupResponseDto>> Get(Guid id)
        => Ok(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<TaskGroupResponseDto>> Create(
        [FromBody] CreateTaskGroupDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<TaskGroupResponseDto>> Update(
        Guid id,
        [FromBody] UpdateTaskGroupDto dto)
    {
        var updated = await service.UpdateAsync(id, dto);
        return Ok(updated);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await service.DeleteAsync(id);
        return NoContent();
    }
}