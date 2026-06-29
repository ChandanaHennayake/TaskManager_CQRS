using MediatR;
using TaskManager.Application.Features.Tasks.DTOs;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Queries.GetAllTasks;

public sealed record GetAllTasksQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Search = null,
    TaskStatus? Status = null,
    string? SortBy = null
) : IRequest<List<TaskDto>>;