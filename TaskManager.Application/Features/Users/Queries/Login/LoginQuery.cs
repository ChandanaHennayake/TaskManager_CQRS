using MediatR;
using TaskManager.Application.Features.Users.DTOs;

namespace TaskManager.Application.Features.Users.Queries.Login;

public sealed record LoginQuery(
    string Username,
    string Password
) : IRequest<UserDto>;