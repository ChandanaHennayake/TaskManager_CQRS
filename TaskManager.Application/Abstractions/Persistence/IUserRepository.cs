using TaskManager.Domain.Entities;

namespace TaskManager.Application.Abstractions.Persistence;

public interface IUserRepository
{
    Task<List<User>> GetAllAsync(
        CancellationToken cancellationToken);

    Task<User?> GetByIdAsync(
        int id,
        CancellationToken cancellationToken);

    Task<User?> GetByUsernameAsync(
        string username,
        CancellationToken cancellationToken);

    Task AddAsync(
        User user,
        CancellationToken cancellationToken);

    void Update(User user);

    void Delete(User user);
}