using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class ProjectService(AppDbContext db) : IProjectService
{
    public async Task<IEnumerable<Project>> GetAllAsync()
    {
        return await db.Projects
            .AsNoTracking()
            .ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Project> CreateAsync(Project project)
    {
        await db.Projects.AddAsync(project);  
        await db.SaveChangesAsync();         
        return project;               
    }
    
    public async Task<Project?> UpdateAsync(Guid id, Project project)
    {
        var existingProject = await db.Projects.FindAsync(id);
        if (existingProject == null) 
            return null; 
        
        existingProject.Name = project.Name;
        existingProject.Description = project.Description;
        existingProject.ProjectLeadId = project.ProjectLeadId;
        existingProject.ProjectManagerId = project.ProjectManagerId;

        await db.SaveChangesAsync();
        return existingProject;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var existingProject = await db.Projects.FindAsync(id);
        if (existingProject == null) 
            return false;

        var tasks = existingProject.Tasks;
        if (tasks.Count > 0 || tasks.Any(t => t.Status == TaskStatus.Cancelled))
            return false;
        
        db.Projects.Remove(existingProject);
        await db.SaveChangesAsync();
        return true;
    }
}