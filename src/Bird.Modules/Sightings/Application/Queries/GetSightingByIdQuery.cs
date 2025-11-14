using Bird.Modules.Sightings.Domain.Entities;
using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Application.Queries;

public record GetSightingByIdQuery(string Id);

public class GetSightingByIdHandler
{
    private readonly ISightingRepository _repository;

    public GetSightingByIdHandler(ISightingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Sighting?> HandleAsync(GetSightingByIdQuery query)
    {
        return await _repository.GetByIdAsync(query.Id);
    }
}
