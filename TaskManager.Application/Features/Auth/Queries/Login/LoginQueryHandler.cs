using BCrypt.Net;
using MediatR;
using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Application.Exceptions;

namespace TaskManager.Application.Features.Auth.Queries.Login;

public sealed class LoginQueryHandler
    : IRequestHandler<LoginQuery, LoginResponse>
{
    private readonly IUserRepository _userRepository;

    public LoginQueryHandler(
        IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<LoginResponse> Handle(
        LoginQuery request,
        CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByUsernameAsync(
            request.Username,
            cancellationToken);

        if (user is null)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        var isPasswordValid = BCrypt.Net.BCrypt.Verify(
            request.Password,
            user.PasswordHash);

        if (!isPasswordValid)
        {
            throw new UnauthorizedAccessException(
                "Invalid username or password.");
        }

        return new LoginResponse
        {
            UserId = user.Id,
            Username = user.Username,
            Email = user.Email
        };
    }
}