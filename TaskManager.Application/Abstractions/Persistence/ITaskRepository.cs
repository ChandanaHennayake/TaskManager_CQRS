using TaskManager.Domain.Entities;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Abstractions.Persistence;

public interface ITaskRepository
{
    Task<List<TaskItem>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<List<TaskItem>> GetPagedTasksAsync(
        int pageNumber,
        int pageSize,
        string? search,
        TaskStatus? status,
        string? sortBy,
        CancellationToken cancellationToken);

    Task<TaskItem?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task AddAsync(
        TaskItem task,
        CancellationToken cancellationToken);

    void Update(TaskItem task);

    void Delete(TaskItem task);
}