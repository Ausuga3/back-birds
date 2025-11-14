using Bird.Modules.Sightings.Domain.Entities;
using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Application.Commands;

public record UpdateSightingCommand(
    string Id,
    double? Latitude,
    double? Longitude,
    string? Country,
    string? BirdId,
    DateTime? Date,
    string? Notes
);

public class UpdateSightingHandler
{
    private readonly ISightingRepository _repository;

    public UpdateSightingHandler(ISightingRepository repository)
    {
        _repository = repository;
    }

    public async Task<Sighting> HandleAsync(UpdateSightingCommand command)
    {
        var existing = await _repository.GetByIdAsync(command.Id);
        if (existing == null)
        {
            throw new Exception("Sighting not found");
        }

        if (command.Latitude.HasValue)
            existing.Latitude = command.Latitude.Value;
        
        if (command.Longitude.HasValue)
            existing.Longitude = command.Longitude.Value;
        
        if (!string.IsNullOrEmpty(command.Country))
            existing.Country = command.Country;
        
        if (!string.IsNullOrEmpty(command.BirdId))
            existing.BirdId = command.BirdId;
        
        if (command.Date.HasValue)
        {
            existing.CreatedAt = command.Date.Value;
        }
        
        if (command.Notes != null)
            existing.Notes = command.Notes;

        existing.UpdatedAt = DateTime.UtcNow;

        return await _repository.UpdateAsync(existing);
    }
}
