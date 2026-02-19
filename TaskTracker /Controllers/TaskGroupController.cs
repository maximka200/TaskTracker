using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;

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
            ? NotFound(new ProblemDetails { Title = "Group not found", Status = StatusCodes.Status404NotFound })
            : Ok(group);
    }

    [HttpPost]
    public async Task<IActionResult> Create(TaskGroup group)
        => Ok(await service.CreateAsync(group));

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, TaskGroup group)
        => Ok(await service.UpdateAsync(id, group));

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
                Status = StatusCodes.Status400BadRequest
            });
    }
}
