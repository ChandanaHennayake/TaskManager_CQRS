using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.Tasks.Commands.DeleteTask;

public class DeleteTaskHandler
    : IRequestHandler<DeleteTaskCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteTaskHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(
        DeleteTaskCommand request,
        CancellationToken cancellationToken)
    {
        var task = await _unitOfWork.Tasks
            .GetByIdAsync(request.Id, cancellationToken);

        if (task is null)
        {
            throw new NotFoundException(
                $"Task with Id {request.Id} was not found.");
        }

        task.IsDeleted = true;
        task.DeletedBy = "admin"; // later get from logged user
        task.DeletedDate = DateTime.UtcNow;

        _unitOfWork.Tasks.Update(task);

        await _unitOfWork.SaveChangesAsync(cancellationToken);

       

        return true;
    }
}