using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.CreateBird;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.UpdateBird;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Queries.GetAllBirds;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Enums;
using System.Security.Claims;
using System.Text.Json;

namespace BackBird.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BirdsController : ControllerBase
    {
        private readonly IBirdRepository _birdRepository;
        private readonly ILogger<BirdsController> _logger;

        public BirdsController(IBirdRepository birdRepository, ILogger<BirdsController> logger)
        {
            _birdRepository = birdRepository;
            _logger = logger;
        }

        // GET api/birds
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            try
            {
                var handler = new GetAllBirdsHandler(_birdRepository);
                var birds = await handler.Handle(new GetAllBirdsQuery());

                var response = birds.Select(b => new
                {
                    id = b.Id,
                    commonName = b.CommonName,
                    scientificName = b.ScientificName,
                    family = b.Family.ToString(),
                    notes = b.Notes,
                    conservationStatus = b.ConservationStatus.ToString(),
                    createdAt = b.Created_At,
                    updatedAt = b.Updated_At,
                    createdBy = b.Created_By
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo todas las aves");
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // GET api/birds/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var bird = await _birdRepository.GetByIdAsync(id);
                if (bird == null)
                {
                    return NotFound($"Ave con ID {id} no encontrada");
                }

                var response = new
                {
                    id = bird.Id,
                    commonName = bird.CommonName,
                    scientificName = bird.ScientificName,
                    family = bird.Family.ToString(),
                    notes = bird.Notes,
                    conservationStatus = bird.ConservationStatus.ToString(),
                    createdAt = bird.Created_At,
                    updatedAt = bird.Updated_At,
                    createdBy = bird.Created_By
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error obteniendo ave {BirdId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }

        // POST api/birds
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateBirdCommand command)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                var handler = new CreateBirdHandler(_birdRepository);
                var bird = await handler.Handle(command, userIdClaim);

                var response = new
                {
                    id = bird.Id,
                    commonName = bird.CommonName,
                    scientificName = bird.ScientificName,
                    family = bird.Family.ToString(),
                    notes = bird.Notes,
                    conservationStatus = bird.ConservationStatus.ToString(),
                    createdAt = bird.Created_At,
                    updatedAt = bird.Updated_At,
                    createdBy = bird.Created_By
                };

                return Created($"/api/birds/{bird.Id}", response);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creando ave");
                return BadRequest(ex.Message);
            }
        }

        // PUT api/birds/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBirdCommand command)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                bool isAdmin = roleClaim?.ToLower() == "admin";

                command.Id = id;
                var handler = new UpdateBirdHandler(_birdRepository);
                var bird = await handler.Handle(command, userIdClaim, isAdmin);

                var response = new
                {
                    id = bird.Id,
                    commonName = bird.CommonName,
                    scientificName = bird.ScientificName,
                    family = bird.Family.ToString(),
                    notes = bird.Notes,
                    conservationStatus = bird.ConservationStatus.ToString(),
                    createdAt = bird.Created_At,
                    updatedAt = bird.Updated_At,
                    createdBy = bird.Created_By
                };

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Forbid(ex.Message);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error actualizando ave {BirdId}", id);
                return BadRequest(ex.Message);
            }
        }

        // DELETE api/birds/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var roleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(userIdClaim))
                {
                    return Unauthorized("Usuario no autenticado");
                }

                bool isAdmin = roleClaim?.ToLower() == "admin";
                var bird = await _birdRepository.GetByIdAsync(id);

                if (bird == null)
                {
                    return NotFound($"Ave con ID {id} no encontrada");
                }

                if (!isAdmin && bird.Created_By != userIdClaim)
                {
                    return Forbid("No tienes permisos para eliminar esta ave");
                }

                await _birdRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error eliminando ave {BirdId}", id);
                return StatusCode(500, "Error interno del servidor");
            }
        }
    }
}
