using TaskTracker.Models;

namespace TaskTracker.Controllers.Interfaces;

public interface IPdfService
{
    byte[] GenerateTaskReport(TaskItem? task);
    byte[] GenerateTaskGroupReport(TaskGroup? group);
}