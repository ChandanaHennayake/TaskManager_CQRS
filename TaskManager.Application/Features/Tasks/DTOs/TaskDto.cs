using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.DTOs;

public sealed class TaskDto
{
    public int Id { get; init; }

    public string Title { get; init; } = string.Empty;

    public string? Description { get; init; }

    public TaskStatus Status { get; init; }

    public DateTime CreatedDate { get; init; }

    public DateTime? UpdatedDate { get; init; }
}