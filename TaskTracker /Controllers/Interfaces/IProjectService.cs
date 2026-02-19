using TaskTracker.Models;

namespace TaskTracker.Controllers.Interfaces;

public interface IProjectService
{
    Task<IEnumerable<Project>> GetAllAsync();
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project> CreateAsync(Project project);
    Task<Project?> UpdateAsync(Guid id, Project project);
    Task<bool> DeleteAsync(Guid id);
}
