using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.CreateBird;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.UpdateBird;
using BackBird.Api.src.Bird.Modules.Birds.Aplication.Queries.GetAllBirds;

namespace BackBird.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class BirdsController : ControllerBase
    {
        private readonly CreateBirdHandler _createBirdHandler;
        private readonly UpdateBirdHandler _updateBirdHandler;
        private readonly GetAllBirdsHandler _getAllBirdsHandler;

        public BirdsController(
            CreateBirdHandler createBirdHandler,
            UpdateBirdHandler updateBirdHandler,
            GetAllBirdsHandler getAllBirdsHandler)
        {
            _createBirdHandler = createBirdHandler;
            _updateBirdHandler = updateBirdHandler;
            _getAllBirdsHandler = getAllBirdsHandler;
        }

        /// <summary>
        /// Obtiene todas las aves registradas
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var query = new GetAllBirdsQuery();
            var birds = await _getAllBirdsHandler.Handle(query);
            return Ok(birds);
        }

        /// <summary>
        /// Crea un nuevo registro de ave
        /// </summary>
        [HttpPost]
        // [Authorize] // Descomenta en producción para proteger el endpoint
        public async Task<IActionResult> Create([FromBody] CreateBirdCommand command)
        {
            // Extraer el ID del usuario autenticado del token JWT (claim)
            // Si no hay usuario autenticado, usar "system" como fallback
            var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "system";

            // Ejecutar el caso de uso
            var bird = await _createBirdHandler.Handle(command, userId);

            // Retornar 201 Created con el ave completa
            return CreatedAtAction(
                actionName: nameof(GetById),
                routeValues: new { id = bird.Id },
                value: bird
            );
        }

        /// <summary>
        /// Actualiza un ave existente
        /// Solo el creador del registro o un admin pueden editarla
        /// </summary>
        [HttpPut("{id}")]
        [Authorize] // Requiere autenticación
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBirdCommand command)
        {
            try
            {
                // Asegurar que el ID del comando coincida con el de la ruta
                command.Id = id;

                // Extraer userId del token JWT
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                    ?? throw new UnauthorizedAccessException("Usuario no autenticado");

                // Verificar si el usuario es admin (case-insensitive)
                var roles = User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value.ToLower());
                var isAdmin = roles.Contains("admin");

                // Ejecutar el caso de uso con validación de permisos
                var bird = await _updateBirdHandler.Handle(command, userId, isAdmin);

                return Ok(bird);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (UnauthorizedAccessException)
            {
                return Forbid(); // 403 Forbidden - usuario autenticado pero sin permisos
            }
        }

        /// <summary>
        /// Obtiene un ave por su ID (placeholder para el CreatedAtAction)
        /// </summary>
        [HttpGet("{id}")]
        public IActionResult GetById(Guid id)
        {
            // TODO: Implementar GetByIdHandler
            return Ok(new { id, message = "GetById not implemented yet" });
        }
    }
}
