using Microsoft.EntityFrameworkCore;
using TaskTracker.Models;

namespace TaskTracker.Repository;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
      public DbSet<Employee> Employees => Set<Employee>();
      public DbSet<Project> Projects => Set<Project>();
      public DbSet<TaskGroup> TaskGroups => Set<TaskGroup>();
      public DbSet<TaskItem> Tasks => Set<TaskItem>();

      protected override void OnModelCreating(ModelBuilder modelBuilder)
      {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<TaskItem>(entity =>
            {
                  entity.HasKey(t => t.Id);

                  entity.Property(t => t.Title)
                        .IsRequired()
                        .HasMaxLength(300);

                  entity.Property(t => t.Description)
                        .HasMaxLength(5000);

                  entity.HasOne(t => t.Project)
                        .WithMany()
                        .HasForeignKey(t => t.ProjectId)
                        .OnDelete(DeleteBehavior.Restrict);

                  entity.HasOne(t => t.TaskGroup)
                        .WithMany()
                        .HasForeignKey(t => t.TaskGroupId)
                        .OnDelete(DeleteBehavior.Restrict);
                  
                  entity.HasMany(t => t.Executors)
                        .WithOne(te => te.TaskItem)
                        .HasForeignKey(te => te.TaskItemId)
                        .OnDelete(DeleteBehavior.Cascade);

                  modelBuilder.Entity<TaskExecutor>(join =>
                  {
                        join.HasKey(te => new { te.TaskItemId, te.EmployeeId });

                        join.HasOne(te => te.Employee)
                              .WithMany()
                              .HasForeignKey(te => te.EmployeeId)
                              .OnDelete(DeleteBehavior.Cascade);
                  });
                  
                  entity.HasMany(t => t.Observers)
                        .WithOne(to => to.TaskItem)
                        .HasForeignKey(to => to.TaskItemId)
                        .OnDelete(DeleteBehavior.Cascade);

                  modelBuilder.Entity<TaskObserver>(join =>
                  {
                        join.HasKey(to => new { to.TaskItemId, to.EmployeeId });

                        join.HasOne(to => to.Employee)
                              .WithMany()
                              .HasForeignKey(to => to.EmployeeId)
                              .OnDelete(DeleteBehavior.Cascade);
                  });
            });
      }
}
