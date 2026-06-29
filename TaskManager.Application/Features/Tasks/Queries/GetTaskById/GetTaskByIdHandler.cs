using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Tasks.DTOs;

namespace TaskManager.Application.Features.Tasks.Queries.GetTaskById;

public class GetTaskByIdHandler
    : IRequestHandler<GetTaskByIdQuery, TaskDto>
{
    private readonly ITaskRepository _taskRepository;

    public GetTaskByIdHandler(
        ITaskRepository taskRepository)
    {
        _taskRepository = taskRepository;
    }

    public async Task<TaskDto> Handle(
        GetTaskByIdQuery request,
        CancellationToken cancellationToken)
    {
        var task = await _taskRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (task is null)
        {
            throw new NotFoundException(
                $"Task with Id {request.Id} was not found.");
        }

        return new TaskDto
        {
            Id = task.Id,
            Title = task.TaskName,
            Description = task.Description,
            Status = task.Status,
            CreatedDate = task.CreatedDate,
            UpdatedDate = task.UpdatedDate
        };
    }
}