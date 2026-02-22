using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.TaskHistory;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/history")]
public class TaskHistoryController(ITaskHistoryService service) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaskHistoryDto>>> GetHistory(
        [FromQuery] Guid? taskId)
    {
        var history = await service.GetHistoryAsync(taskId);

        return Ok(history);
    }
}