using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;

namespace BackBird.Api.src.Bird.Modules.Users.Domain.Repositories
{
    public interface IUserRepository
    {
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(Guid id);
        Task AddAsync(User user);
    }
}
