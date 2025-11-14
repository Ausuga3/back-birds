using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using BackBird.Api.src.Bird.Modules.Users.Aplication.Models.Dto;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence;
using BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.UpdateUser;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;
using BackBird.Api.src.Bird.Modules.Users.Domain.Enums;
using System.Security.Claims;

namespace BackBird.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersDbContext _db;
        private readonly IUserRepository _userRepository;
        private readonly ILogger<UsersController> _logger;

        public UsersController(UsersDbContext db, IUserRepository userRepository, ILogger<UsersController> logger)
        {
            _db = db;
            _userRepository = userRepository;
            _logger = logger;
        }

        // GET api/Users
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userRepository.GetAllAsync();
            var dtos = users.Select(user => new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RolName = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.Created_At
            }).ToList();

            return Ok(dtos);
        }

        // GET api/Users/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Get(Guid id)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null) return NotFound();

            var dto = new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                RolName = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.Created_At
            };

            return Ok(dto);
        }

        // PUT api/Users/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserCommand command)
        {
            try
            {
                // Log de headers y claims
                var authHeader = Request.Headers["Authorization"].ToString();
                _logger.LogInformation($"[UsersController] Authorization Header: {(string.IsNullOrEmpty(authHeader) ? "MISSING" : "Present")}");
                _logger.LogInformation($"[UsersController] User.Identity.IsAuthenticated: {User.Identity?.IsAuthenticated}");
                
                // Obtener el usuario autenticado
                var actorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var actorRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                _logger.LogInformation($"[UsersController] UPDATE User {id} - Actor: {actorIdClaim}, Role: {actorRoleClaim}");

                if (string.IsNullOrEmpty(actorIdClaim) || string.IsNullOrEmpty(actorRoleClaim))
                {
                    return Unauthorized(new { message = "No se pudo identificar al usuario autenticado" });
                }

                if (!Guid.TryParse(actorIdClaim, out Guid actorId))
                {
                    return BadRequest("ID de usuario inválido");
                }

                // Parsear el rol del claim (viene en minúsculas: "admin", "experto", "usuario")
                Role actorRole;
                if (actorRoleClaim.ToLower() == "admin")
                {
                    actorRole = Role.Admin;
                }
                else if (actorRoleClaim.ToLower() == "experto")
                {
                    actorRole = Role.Experto;
                }
                else
                {
                    actorRole = Role.Usuario;
                }

                // Ejecutar el handler
                var handler = new UpdateUserHandler(_userRepository);
                var updatedUser = await handler.Handle(id, command, actorId, actorRole);

                // Mapear a DTO
                var dto = new UserDto
                {
                    Id = updatedUser.Id,
                    Name = updatedUser.Name,
                    Email = updatedUser.Email,
                    RolName = updatedUser.Role.ToString(),
                    IsActive = updatedUser.IsActive,
                    CreatedAt = updatedUser.Created_At
                };

                return Ok(dto);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
