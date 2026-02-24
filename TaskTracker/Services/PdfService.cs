using TaskTracker.Controllers.Interfaces;
using TaskTracker.Models;
using QuestPDF.Fluent;
using Document = QuestPDF.Fluent.Document;

namespace TaskTracker.Services;

public class PdfService : IPdfService
{
    public byte[] GenerateTaskReport(TaskItem? task)
    {
        if (task is null)
            throw new KeyNotFoundException("Task not found");
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Content().Column(column =>
                {
                    column.Spacing(10);

                    column.Item().Text("Task Report").FontSize(20).Bold();
                    column.Item().Text($"Title: {task.Title}");
                    column.Item().Text($"Description: {task.Description}");
                    column.Item().Text($"Status: {task.Status}");
                    column.Item().Text($"Priority: {task.Priority}");
                    column.Item().Text($"Created At: {task.CreatedAt:yyyy-MM-dd HH:mm}");
                    column.Item().Text($"Due Date: {task.DueDate:yyyy-MM-dd HH:mm}");
                    column.Item().Text($"Project: {task.Project.Name}");
                    column.Item().Text($"Group: {task.TaskGroup.Name}");
                    
                    column.Item().PaddingTop(10).Text("Executors:").Bold();
                    foreach (var executor in task.Executors)
                    {
                        column.Item().Text($"- {executor.Employee?.FirstName} {executor.Employee?.LastName}");
                    }

                    column.Item().PaddingTop(10).Text("Observers:").Bold();
                    foreach (var observer in task.Observers)
                    {
                        column.Item().Text($"- {observer.Employee?.FirstName} {observer.Employee?.LastName}");
                    }
                });
            });
        });

        return document.GeneratePdf();
    }
    
    public byte[] GenerateTaskGroupReport(TaskGroup? group)
    {
        if (group is null)
            throw new KeyNotFoundException("Group not found");
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);

                page.Content().Column(column =>
                {
                    column.Spacing(8);

                    column.Item().Text("Task Group Report").FontSize(20).Bold();

                    column.Item().Text($"Group: {group.Name}");
                    column.Item().Text($"Project: {group.Project?.Name}");
                    column.Item().Text($"Total Tasks: {group.Tasks.Count}");

                    column.Item().PaddingTop(10).Text("Tasks:").Bold();

                    foreach (var task in group.Tasks)
                    {
                        column.Item().PaddingTop(5).Text($"Task: {task.Title}").Bold();

                        column.Item().Text($"Description: {task.Description}");
                        column.Item().Text($"Status: {task.Status}");
                        column.Item().Text($"Priority: {task.Priority}");
                        column.Item().Text($"Due: {task.DueDate:yyyy-MM-dd}");
                        column.Item().Text($"Created: {task.CreatedAt:yyyy-MM-dd}");

                        column.Item().Text("Executors:");
                        foreach (var executor in task.Executors)
                        {
                            column.Item().Text($" - {executor.Employee?.FirstName} {executor.Employee?.LastName}");
                        }

                        column.Item().Text("Observers:");
                        foreach (var observer in task.Observers)
                        {
                            column.Item().Text($" - {observer.Employee?.FirstName} {observer.Employee?.LastName}");
                        }

                        column.Item().LineHorizontal(1);
                    }
                });
            });
        });

        return document.GeneratePdf();
    }
}