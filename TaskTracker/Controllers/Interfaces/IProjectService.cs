using TaskTracker.Models.DTOs.Project;

namespace TaskTracker.Controllers.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<ProjectResponseDto>> GetAllAsync();
    Task<ProjectResponseDto?> GetByIdAsync(Guid id);
    Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto);
    Task<ProjectResponseDto?> UpdateAsync(Guid id, UpdateProjectDto dto);
    Task<bool> DeleteAsync(Guid id);
}