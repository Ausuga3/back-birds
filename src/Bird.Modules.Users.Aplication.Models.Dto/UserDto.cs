using System;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Models.Dto
{
 public class UserDto
 {
 public Guid Id { get; set; }
 public string Name { get; set; } = string.Empty;
 public string Email { get; set; } = string.Empty;
 public string RolName { get; set; } = string.Empty;
 public bool IsActive { get; set; }
 public DateTime? CreatedAt { get; set; }
 }
}
