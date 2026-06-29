using MediatR;
using TaskManager.Application.Features.Tasks.DTOs;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public record GetTaskByIdQuery(int Id) : IRequest<TaskDto?>;