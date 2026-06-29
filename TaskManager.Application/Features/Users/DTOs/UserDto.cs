namespace TaskManager.Application.Features.Users.DTOs;

public sealed record UserDto(
    int Id,
    string Username,
    string Email
);

