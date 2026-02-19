using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var employees = await employeeService.GetAllAsync();
        return Ok(employees);
    }
    
    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var employee = await employeeService.GetByIdAsync(id);
        if (employee == null)
            return NotFound(new ProblemDetails
            {
                Title = "Employee not found or id not correct",
                Status = StatusCodes.Status404NotFound
            });

        return Ok(employee);
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] Employee employee)
    {
        var created = await employeeService.CreateAsync(employee);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }
    
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] Employee employee)
    {
        var updated = await employeeService.UpdateAsync(id, employee);
        if (updated == null)
            return NotFound(new ProblemDetails
            {
                Title = "Employee not found or id not correct",
                Status = StatusCodes.Status404NotFound
            });

        return Ok(updated);
    }
    
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await employeeService.DeleteAsync(id);
        if (!deleted)
            return NotFound(new ProblemDetails
            {
                Title = "Employee not found or id not correct",
                Status = StatusCodes.Status404NotFound
            });

        return NoContent();
    }
}
