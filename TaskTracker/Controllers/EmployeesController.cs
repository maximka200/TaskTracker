using Microsoft.AspNetCore.Mvc;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models.DTOs.Employee;

namespace TaskTracker.Controllers;

[ApiController]
[Route("api/employees")]
public class EmployeesController(IEmployeeService employeeService) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<EmployeeResponseDto>>> GetAll()
        => Ok(await employeeService.GetAllAsync());

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> GetById(Guid id)
        => Ok(await employeeService.GetByIdAsync(id));

    [HttpPost]
    public async Task<ActionResult<EmployeeResponseDto>> Create(
        [FromBody] CreateEmployeeDto dto)
    {
        var created = await employeeService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:guid}")]
    public async Task<ActionResult<EmployeeResponseDto>> Update(
        Guid id,
        [FromBody] UpdateEmployeeDto dto)
        => Ok(await employeeService.UpdateAsync(id, dto));

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        await employeeService.DeleteAsync(id);
        return NoContent();
    }
}