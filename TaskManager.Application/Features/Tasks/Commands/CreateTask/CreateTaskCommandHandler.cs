using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Domain.Entities;

namespace TaskManager.Application.Features.Tasks.Commands.CreateTask;

public class CreateTaskCommandHandler
    : IRequestHandler<CreateTaskCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTaskCommandHandler(
        IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<int> Handle(
        CreateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = new TaskItem
        {
            TaskName = request.Title.Trim(),
            Description = request.Description.Trim(),
            Status = request.Status,
            CreatedDate = DateTime.UtcNow
        };

        await _unitOfWork.Tasks.AddAsync(
            task,
            cancellationToken);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return task.Id;
    }
}