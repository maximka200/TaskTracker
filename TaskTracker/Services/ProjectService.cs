using Microsoft.EntityFrameworkCore;
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using TaskTracker.Models.DTOs.Project;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Services;

public class ProjectService(AppDbContext db) : IProjectService
{ 
    public async Task<IEnumerable<ProjectResponseDto>> GetAllAsync()
    {
        return await db.Projects
            .AsNoTracking()
            .Select(p => new ProjectResponseDto
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                ProjectLeadId = p.ProjectLeadId,
                ProjectManagerId = p.ProjectManagerId,
            })
            .ToListAsync();
    }
    
    public async Task<ProjectResponseDto?> GetByIdAsync(Guid id)
    {
        var project = await db.Projects
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id);

        if (project == null) return null;

        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            ProjectLeadId = project.ProjectLeadId,
            ProjectManagerId = project.ProjectManagerId
        };
    }
    
    public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto)
    {
        var project = new Project
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            Description = dto.Description,
            ProjectLeadId = dto.ProjectLeadId,
            ProjectManagerId = dto.ProjectManagerId
        };

        await db.Projects.AddAsync(project);
        await db.SaveChangesAsync();

        return new ProjectResponseDto
        {
            Id = project.Id,
            Name = project.Name,
            Description = project.Description,
            ProjectLeadId = project.ProjectLeadId,
            ProjectManagerId = project.ProjectManagerId
        };
    }
    
    public async Task<ProjectResponseDto?> UpdateAsync(Guid id, UpdateProjectDto dto)
    {
        var existingProject = await db.Projects.FindAsync(id);
        if (existingProject == null) return null;
        
        if (dto.Name != null)
            existingProject.Name = dto.Name;
        if (dto.Description != null)
            existingProject.Description = dto.Description;
        if (dto.ProjectLeadId != null)
            existingProject.ProjectLeadId = dto.ProjectLeadId.Value;
        if (dto.ProjectManagerId != null)
            existingProject.ProjectManagerId = dto.ProjectManagerId.Value;

        await db.SaveChangesAsync();

        return new ProjectResponseDto
        {
            Id = existingProject.Id,
            Name = existingProject.Name,
            Description = existingProject.Description,
            ProjectLeadId = existingProject.ProjectLeadId,
            ProjectManagerId = existingProject.ProjectManagerId
        };
    }
    
    public async Task<bool> DeleteAsync(Guid id)
    {
        var existingProject = await db.Projects
            .Include(p => p.Tasks)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (existingProject == null) return false;

        if (existingProject.Tasks.Count > 0 || existingProject.Tasks.Any(t => t.Status == TaskStatus.Cancelled))
            return false;

        db.Projects.Remove(existingProject);
        await db.SaveChangesAsync();
        return true;
    }
}