using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.Project;

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
            ? NotFound(new ProblemDetails 
                { Title = "Project not found", Status = 404 })
            : Ok(project);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateProjectDto dto)
        => Ok(await service.CreateAsync(dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, UpdateProjectDto dto)
        => Ok(await service.UpdateAsync(id, dto));

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
                Status = 400
            });
    }
}
