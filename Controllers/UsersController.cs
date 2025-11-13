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

        public UsersController(UsersDbContext db, IUserRepository userRepository)
        {
            _db = db;
            _userRepository = userRepository;
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
                IsActive = true, // TODO: Agregar IsActive a la entidad User si lo necesitas
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
                IsActive = true,
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
                // Obtener el usuario autenticado
                var actorIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var actorRoleClaim = User.FindFirst(ClaimTypes.Role)?.Value;

                if (string.IsNullOrEmpty(actorIdClaim) || string.IsNullOrEmpty(actorRoleClaim))
                {
                    return Unauthorized("No se pudo identificar al usuario autenticado");
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
                    IsActive = true,
                    CreatedAt = updatedUser.Created_At
                };

                return Ok(dto);
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
                return StatusCode(500, $"Error interno: {ex.Message}");
            }
        }
    }
}
