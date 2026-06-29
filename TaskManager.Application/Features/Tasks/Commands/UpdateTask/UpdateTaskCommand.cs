using MediatR;
using TaskManager.Domain.Enums;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public sealed record UpdateTaskCommand(
    int Id,
    string Title,
    string Description,
    TaskStatus Status
) : IRequest<bool>;