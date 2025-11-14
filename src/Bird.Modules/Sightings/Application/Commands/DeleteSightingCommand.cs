using Bird.Modules.Sightings.Domain.Repositories;

namespace Bird.Modules.Sightings.Application.Commands;

public record DeleteSightingCommand(string Id);

public class DeleteSightingHandler
{
    private readonly ISightingRepository _repository;

    public DeleteSightingHandler(ISightingRepository repository)
    {
        _repository = repository;
    }

    public async Task HandleAsync(DeleteSightingCommand command)
    {
        var existing = await _repository.GetByIdAsync(command.Id);
        if (existing == null)
        {
            throw new Exception("Sighting not found");
        }

        await _repository.DeleteAsync(command.Id);
    }
}
