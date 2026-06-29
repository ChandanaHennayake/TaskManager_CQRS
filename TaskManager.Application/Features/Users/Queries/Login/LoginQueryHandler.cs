using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Abstractions.Security;
using TaskManager.Application.Exceptions;
using TaskManager.Application.Features.Users.DTOs;

namespace TaskManager.Application.Features.Users.Queries.Login;

public class LoginQueryHandler
    : IRequestHandler<LoginQuery, UserDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHasher _passwordHasher;

    public LoginQueryHandler(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher)
    {
        _unitOfWork = unitOfWork;
        _passwordHasher = passwordHasher;
    }

    public async Task<UserDto> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user =
            await _unitOfWork.Users.GetByUsernameAsync(
                request.Username.Trim(),
                cancellationToken);

        if (user is null)
        {
            throw new NotFoundException(
                $"User '{request.Username}' not found");
        }

        var isValid =
            _passwordHasher.VerifyPassword(
                request.Password,
                user.PasswordHash);

        if (!isValid)
        {
            throw new UnauthorizedException(
                "Invalid credentials");
        }

        return new UserDto(
            user.Id,
            user.Username,
            user.Email);
    }
}