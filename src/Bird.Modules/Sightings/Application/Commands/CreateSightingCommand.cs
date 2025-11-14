using Bird.Modules.Sightings.Domain.Entities;
using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Application.Commands;

public record CreateSightingCommand(
    double Latitude,
    double Longitude,
    string Country,
    string BirdId,
    DateTime Date,
    string? Notes,
    string? CreatedBy
);

public class CreateSightingHandler
{
    private readonly ISightingRepository _repository;

    public CreateSightingHandler(ISightingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Sighting> HandleAsync(CreateSightingCommand command)
    {
        var sighting = new Sighting
        {
            Latitude = command.Latitude,
            Longitude = command.Longitude,
            Country = command.Country,
            BirdId = command.BirdId,
            Notes = command.Notes,
            CreatedBy = command.CreatedBy,
            CreatedAt = command.Date,
            UpdatedAt = command.Date
        };

        return await _repository.AddAsync(sighting);
    }
}
