using Microsoft.EntityFrameworkCore;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Persistence.DBSeed;

public static class TaskSeeder
{
    public static async Task SeedAsync(ApplicationDbContext context)
    {
        if (await context.Tasks.AnyAsync())
        {
            return;
        }

        var tasks = new List<TaskItem>
        {
            new()
            {
                TaskName = "Design Database",
                Description = "Create SQL Server database schema.",
                Status = (TaskStatus)TaskItemStatus.Pending
            },
            new()
            {
                TaskName = "Implement Authentication",
                Description = "Develop login functionality.",
                Status = (TaskStatus)TaskItemStatus.InProgress
            },
            new()
            {
                TaskName = "Develop Task CRUD",
                Description = "Implement Create, Read, Update and Delete operations.",
                Status = (TaskStatus)TaskItemStatus.Completed
            },
            new()
            {
                TaskName = "Build Angular Frontend",
                Description = "Develop responsive task management screens.",
                Status = (TaskStatus)TaskItemStatus.Pending
            },
            new()
            {
                TaskName = "Write Unit Tests",
                Description = "Cover application layer with unit tests.",
                Status = (TaskStatus)TaskItemStatus.InProgress
            }
        };

        await context.Tasks.AddRangeAsync(tasks);
        await context.SaveChangesAsync();
    }
}