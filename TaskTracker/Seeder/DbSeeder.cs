using TaskTracker.Models;
using TaskTracker.Repository;
using TaskStatus = TaskTracker.Models.TaskStatus;

namespace TaskTracker.Seeder;

public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (db.Employees.Any())
            return;

        var random = new Random();
        var employees = new List<Employee>();

        for (var i = 1; i <= 30; i++)
        {
            employees.Add(new Employee
            (
                $"Last{i}",
                 $"First{i}",
                $"Middle{i}",
                $"User{i}",
                (EmployeeRole)random.Next(0, 4)
            ));
        }

        await db.Employees.AddRangeAsync(employees);
        var projects = new List<Project>();

        for (var i = 1; i <= 30; i++)
        {
            projects.Add(new Project
            {
                Id = Guid.NewGuid(),
                Name = $"Project {i}",
                Description = $"Description {i}",
                ProjectManager = employees[random.Next(employees.Count)],
                ProjectLead = employees[random.Next(employees.Count)]
            });
        }

        await db.Projects.AddRangeAsync(projects);
        var groups = new List<TaskGroup>();

        for (var i = 1; i <= 30; i++)
        {
            groups.Add(new TaskGroup
            {
                Id = Guid.NewGuid(),
                Name = $"Group {i}",
                ProjectId = projects[random.Next(projects.Count)].Id
            });
        }

        await db.TaskGroups.AddRangeAsync(groups);
        await db.SaveChangesAsync();
        
        var tasks = new List<TaskItem>();

        for (var i = 1; i <= 40; i++)
        {
            var project = projects[random.Next(projects.Count)];
            var group = groups[random.Next(groups.Count)];

            tasks.Add(new TaskItem
            {
                Id = Guid.NewGuid(),
                Title = $"{i}",
                Description = $"{i}",
                CreatedAt = DateTime.UtcNow.AddDays(-random.Next(30)),
                Status = (TaskStatus)random.Next(0, 4),
                Priority = (TaskPriority)random.Next(0, 3),
                ProjectId = project.Id,
                TaskGroupId = group.Id
            });
        }

        await db.Tasks.AddRangeAsync(tasks);
        await db.SaveChangesAsync();
        
        var executors = new List<TaskExecutor>();
        var observers = new List<TaskObserver>();

        foreach (var task in tasks)
        {
            var shuffledEmployees = employees.OrderBy(_ => random.Next()).Take(3).ToList();

            executors.AddRange(shuffledEmployees.Take(2).Select(e =>
                new TaskExecutor
                {
                    TaskItemId = task.Id,
                    EmployeeId = e.Id
                }));

            observers.AddRange(shuffledEmployees.Skip(2).Select(e =>
                new TaskObserver
                {
                    TaskItemId = task.Id,
                    EmployeeId = e.Id
                }));
        }

        await db.Set<TaskExecutor>().AddRangeAsync(executors);
        await db.Set<TaskObserver>().AddRangeAsync(observers);

        await db.SaveChangesAsync();
    }
}