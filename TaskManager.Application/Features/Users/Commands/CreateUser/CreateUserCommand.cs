using MediatR;

namespace TaskManager.Application.Features.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Username,
    string Email,
    string Password
) : IRequest<int>;