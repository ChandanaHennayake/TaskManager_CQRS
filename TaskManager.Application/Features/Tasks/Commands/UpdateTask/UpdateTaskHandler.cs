using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.Tasks.Commands.UpdateTask;

public class UpdateTaskHandler
    : IRequestHandler<UpdateTaskCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        UpdateTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks
            .GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException(
                $"Task with Id {request.Id} was not found.");
        }

        task.TaskName = request.Title;
        task.Description = request.Description;
        task.Status = request.Status;
        task.UpdatedDate = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}