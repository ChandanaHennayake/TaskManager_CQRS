using MediatR;

namespace TaskManager.Application.Features.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    int Id,
    string Username,
    string Email,
    string Password
) : IRequest<int>;