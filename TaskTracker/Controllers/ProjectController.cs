using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.Project;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProjectResponseDto>>> GetAll()
        => Ok(await service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<ProjectResponseDto>> Get(Guid id)
        => Ok(await service.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<ProjectResponseDto>> Create(
        [FromBody] CreateProjectDto dto)
    {
        var created = await service.CreateAsync(dto);
        return CreatedAtAction(nameof(Get), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<ProjectResponseDto>> Update(
        Guid id,
        [FromBody] UpdateProjectDto dto)
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