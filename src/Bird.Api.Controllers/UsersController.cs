using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using BackBird.Api.src.Bird.Modules.Users.Infrastructure.Persistence;
using BackBird.Api.src.Bird.Modules.Users.Aplication.Models.Dto;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace BackBird.Api.src.Bird.Api.Controllers
{
 [ApiController]
 [Route("api/users")]
 public class UsersController : ControllerBase
 {
 private readonly UsersDbContext _db;
 
 public UsersController(UsersDbContext db)
 {
 _db = db;
 }

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
 }
}
