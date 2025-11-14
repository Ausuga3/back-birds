using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Bird.Modules.Sightings.Application.Commands;
using Bird.Modules.Sightings.Application.Queries;

namespace Bird.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SightingsController : ControllerBase
{
    private readonly CreateSightingHandler _createHandler;
    private readonly UpdateSightingHandler _updateHandler;
    private readonly DeleteSightingHandler _deleteHandler;
    private readonly GetAllSightingsHandler _getAllHandler;
    private readonly GetSightingByIdHandler _getByIdHandler;
    private readonly GetSightingsByBirdIdHandler _getByBirdIdHandler;

    public SightingsController(
        CreateSightingHandler createHandler,
        UpdateSightingHandler updateHandler,
        DeleteSightingHandler deleteHandler,
        GetAllSightingsHandler getAllHandler,
        GetSightingByIdHandler getByIdHandler,
        GetSightingsByBirdIdHandler getByBirdIdHandler)
    {
        _createHandler = createHandler;
        _updateHandler = updateHandler;
        _deleteHandler = deleteHandler;
        _getAllHandler = getAllHandler;
        _getByIdHandler = getByIdHandler;
        _getByBirdIdHandler = getByBirdIdHandler;
    }

    // GET: api/sightings
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var sightings = await _getAllHandler.HandleAsync(new GetAllSightingsQuery());
        return Ok(sightings);
    }

    // GET: api/sightings/{id}
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(string id)
    {
        var sighting = await _getByIdHandler.HandleAsync(new GetSightingByIdQuery(id));
        if (sighting == null)
            return NotFound();

        return Ok(sighting);
    }

    // GET: api/sightings/bird/{birdId}
    [HttpGet("bird/{birdId}")]
    public async Task<IActionResult> GetByBirdId(string birdId)
    {
        var sightings = await _getByBirdIdHandler.HandleAsync(new GetSightingsByBirdIdQuery(birdId));
        return Ok(sightings);
    }

    // POST: api/sightings
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSightingDto dto)
    {
        var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var command = new CreateSightingCommand(
            dto.Coordinates.Latitude,
            dto.Coordinates.Longitude,
            dto.Country,
            dto.BirdId,
            DateTime.Parse(dto.Date),
            dto.Notes,
            userId
        );

        var sighting = await _createHandler.HandleAsync(command);
        return CreatedAtAction(nameof(GetById), new { id = sighting.Id }, sighting);
    }

    // PUT: api/sightings/{id}
    [Authorize]
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateSightingDto dto)
    {
        try
        {
            var existing = await _getByIdHandler.HandleAsync(new GetSightingByIdQuery(id));
            if (existing == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

            // Only admin or creator can update
            if (userRole != "admin" && existing.CreatedBy != userId)
            {
                return StatusCode(403, new { message = "No tienes permisos para actualizar este avistamiento" });
            }

            var command = new UpdateSightingCommand(
                id,
                dto.Coordinates?.Latitude,
                dto.Coordinates?.Longitude,
                dto.Country,
                dto.BirdId,
                dto.Date != null ? DateTime.Parse(dto.Date) : null,
                dto.Notes
            );

            var updated = await _updateHandler.HandleAsync(command);
            return Ok(updated);
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    // DELETE: api/sightings/{id}
    [Authorize]
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id)
    {
        try
        {
            var existing = await _getByIdHandler.HandleAsync(new GetSightingByIdQuery(id));
            if (existing == null)
                return NotFound();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value?.ToLower();

            // Only admin or creator can delete
            if (userRole != "admin" && existing.CreatedBy != userId)
            {
                return StatusCode(403, new { message = "No tienes permisos para eliminar este avistamiento" });
            }

            await _deleteHandler.HandleAsync(new DeleteSightingCommand(id));
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }
}

// DTOs
public class CoordinatesDto
{
    public double Latitude { get; set; }
    public double Longitude { get; set; }
}

public class CreateSightingDto
{
    public CoordinatesDto Coordinates { get; set; } = new();
    public string Country { get; set; } = string.Empty;
    public string BirdId { get; set; } = string.Empty;
    public string Date { get; set; } = string.Empty;
    public string? Notes { get; set; }
}

public class UpdateSightingDto
{
    public CoordinatesDto? Coordinates { get; set; }
    public string? Country { get; set; }
    public string? BirdId { get; set; }
    public string? Date { get; set; }
    public string? Notes { get; set; }
}
