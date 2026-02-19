using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/projects")]
public class ProjectController(IProjectService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var project = await service.GetByIdAsync(id);
        return project is null
            ? NotFound(new ProblemDetails { Title = "Project not found", Status = StatusCodes.Status404NotFound })
            : Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create(Project project)
        => Ok(await service.CreateAsync(project));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, Project project)
        => Ok(await service.UpdateAsync(id, project));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await service.DeleteAsync(id);
        return result
            ? Ok()
            : BadRequest(new ProblemDetails
            {
                Title = "Cannot delete project",
                Detail = "Project contains active tasks",
                Status = StatusCodes.Status400BadRequest
            });
    }
}
