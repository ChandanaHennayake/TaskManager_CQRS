using MediatR;
using TaskManager.Application.Features.Auth.Queries.Login;

namespace TaskManager.Application.Features.Auth.Queries.Login;

public sealed record LoginQuery(
    string Username,
    string Password
) : IRequest<LoginResponse>;