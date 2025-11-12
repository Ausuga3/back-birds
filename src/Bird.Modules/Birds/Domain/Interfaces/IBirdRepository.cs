
using BirdEntity = BackBird.Api.src.Bird.Modules.Birds.Domain.Entities.Bird;

namespace BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories
{
    public interface IBirdRepository
    {
        Task<BirdEntity?> GetByIdAsync(Guid id);
        Task<IEnumerable<BirdEntity>> GetAllAsync();
        Task AddAsync(BirdEntity bird);
        Task UpdateAsync(BirdEntity bird);
        Task DeleteAsync(Guid id);
    }
}
