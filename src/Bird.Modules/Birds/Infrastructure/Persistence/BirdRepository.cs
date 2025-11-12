using Microsoft.EntityFrameworkCore;
using BirdEntity = BackBird.Api.src.Bird.Modules.Birds.Domain.Entities.Bird;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;

namespace BackBird.Api.src.Bird.Modules.Birds.Infrastructure.Persistence
{
    public class BirdRepository : IBirdRepository
    {
        private readonly BirdsDbContext _context;

        public BirdRepository(BirdsDbContext context)
        {
            _context = context;
        }

        public async Task<BirdEntity?> GetByIdAsync(Guid id)
        {
            return await _context.Birds.FindAsync(id);
        }

        public async Task<IEnumerable<BirdEntity>> GetAllAsync()
        {
            return await _context.Birds.ToListAsync();
        }

        public async Task AddAsync(BirdEntity bird)
        {
            await _context.Birds.AddAsync(bird);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(BirdEntity bird)
        {
            _context.Birds.Update(bird);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var bird = await GetByIdAsync(id);
            if (bird != null)
            {
                _context.Birds.Remove(bird);
                await _context.SaveChangesAsync();
            }
        }
    }
}
