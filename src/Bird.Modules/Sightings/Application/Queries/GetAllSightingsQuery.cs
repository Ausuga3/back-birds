using Bird.Modules.Sightings.Domain.Entities;
using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Application.Queries;

public record GetAllSightingsQuery();

public class GetAllSightingsHandler
{
    private readonly ISightingRepository _repository;

    public GetAllSightingsHandler(ISightingRepository repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Sighting>> HandleAsync(GetAllSightingsQuery query)
    {
        return await _repository.GetAllAsync();
    }
}
