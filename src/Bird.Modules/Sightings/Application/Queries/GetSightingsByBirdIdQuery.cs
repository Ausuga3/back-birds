using Bird.Modules.Sightings.Domain.Entities;
using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Application.Queries;

public record GetSightingsByBirdIdQuery(string BirdId);

public class GetSightingsByBirdIdHandler
{
    private readonly ISightingRepository _repository;

    public GetSightingsByBirdIdHandler(ISightingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Sighting>> HandleAsync(GetSightingsByBirdIdQuery query)
    {
        return await _repository.GetByBirdIdAsync(query.BirdId);
    }
}
