using TaskManager.Application.Abstractions.Persistence;
using TaskManager.Infrastructure.Persistence.Repositories;

namespace TaskManager.Infrastructure.Persistence.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public ITaskRepository Tasks { get; }
       public IUserRepository Users { get; }

        public UnitOfWork(
            ApplicationDbContext context)
        {
            _context = context;
            Tasks = new TaskRepository(context);
            Users = new UserRepository(context);

        }

        public async Task<int> SaveChangesAsync(
            CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
    }
}