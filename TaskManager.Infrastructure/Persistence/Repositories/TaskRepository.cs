using Microsoft.EntityFrameworkCore;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Infrastructure.Persistence.Repositories;

public class TaskRepository : ITaskRepository
{
    private readonly ApplicationDbContext _context;

    public TaskRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<TaskItem>> GetAllAsync(
        CancellationToken cancellationToken)
    {
        return await _context.Tasks
    .Where(x => !x.IsDeleted)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task<TaskItem?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken)
    {
        return await _context.Tasks

            .FirstOrDefaultAsync(
                x => x.Id == id,
                cancellationToken);
    }

    public async Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken)
    {
        await _context.Tasks.AddAsync(
            task,
            cancellationToken);
    }

    public void Update(TaskItem task)
    {
        _context.Tasks.Update(task);
    }

    public void Delete(TaskItem task)
    {
        _context.Tasks.Remove(task);
    }

    public async Task<List<TaskItem>> GetPagedTasksAsync(
        int pageNumber,
        int pageSize,
        string? search,
        TaskStatus? status,
        string? sortBy,
        CancellationToken cancellationToken)
    {
        var query = _context.Tasks
            .Where(x => !x.IsDeleted)
            .AsNoTracking()
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(x =>
                x.TaskName.Contains(search));
        }

        if (status.HasValue)
        {
            query = query.Where(x =>
                x.Status == status.Value);
        }

        query = sortBy?.ToLower() switch
        {
            "taskname" => query.OrderBy(x => x.TaskName),
            "createddate" => query.OrderByDescending(x => x.CreatedDate),
            "status" => query.OrderBy(x => x.Status),
            _ => query.OrderByDescending(x => x.Id)
        };

        return await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}