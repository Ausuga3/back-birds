using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BackBird.Api.src.Bird.Modules.Users.Aplication.Models.Dto;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence;

namespace BackBird.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly UsersDbContext _db;

        public UsersController(UsersDbContext db)
        {
            _db = db;
        }

        // GET api/Users/{id}
        // [Authorize] // Uncomment in production to protect this route.
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
                CreatedAt = null
            };

            return Ok(dto);
        }
    }
}
