using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Security;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler
    : IRequestHandler<UpdateUserCommand, int>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public UpdateUserCommandHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<int> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var user =
            await _unitOfWork.Users.GetByIdAsync(
                request.Id,
                cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                $"User {request.Id} not found");
        }

        user.Username = request.Username.Trim();
        user.Email = request.Email.Trim();

        user.PasswordHash =
            _passwordHasher.HashPassword(
                request.Password);

        _unitOfWork.Users.Update(user);

        await _unitOfWork.SaveChangesAsync(
            cancellationToken);

        return user.Id;
    }
}