using Bird.Modules.Sightings.Domain.Entities;

namespace Bird.Modules.Sightings.Domain.Repositories;

public interface ISightingRepository
{
    Task<Sighting> AddAsync(Sighting sighting);
    Task<Sighting?> GetByIdAsync(string id);
    Task<IEnumerable<Sighting>> GetAllAsync();
    Task<IEnumerable<Sighting>> GetByBirdIdAsync(string birdId);
    Task<Sighting> UpdateAsync(Sighting sighting);
    Task DeleteAsync(string id);
}
