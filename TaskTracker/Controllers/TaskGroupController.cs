using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskGroup;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/taskgroups")]
public class TaskGroupController(ITaskGroupService service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
        => Ok(await service.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var group = await service.GetByIdAsync(id);
        return group is null
            ? NotFound(new ProblemDetails
            {
                Title = "Group not found", 
                Status = 404
            })
            : Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateTaskGroupDto dto)
        => Ok(await service.CreateAsync(dto));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTaskGroupDto dto)
        => Ok(await service.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await service.DeleteAsync(id);
        return result
            ? Ok()
            : BadRequest(new ProblemDetails
            {
                Title = "Cannot delete group",
                Detail = "Group contains active tasks",
                Status = 400
            });
    }
}