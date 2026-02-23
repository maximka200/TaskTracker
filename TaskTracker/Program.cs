using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure; 
using TaskTracker.Controllers.Interfaces;
using TaskTracker.Middleware;
using TaskTracker.Repository;
using TaskTracker.Seeder;
using TaskTracker.Services;

var builder = WebApplication.CreateBuilder(args);

QuestPDF.Settings.License = LicenseType.Community;

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<ITaskService, TaskService>();
builder.Services.AddScoped<ITaskGroupService, TaskGroupService>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<ITaskHistoryService, TaskHistoryService>();

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddProblemDetails();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

    await db.Database.MigrateAsync();
    await DbSeeder.SeedAsync(db);
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();