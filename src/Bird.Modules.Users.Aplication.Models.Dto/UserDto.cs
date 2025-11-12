using System;
using System.Text.Json.Serialization;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Models.Dto
{
 public class UserDto
 {
 [JsonPropertyName("id")]
 public Guid Id { get; set; }
 
 [JsonPropertyName("name")]
 public string Name { get; set; } = string.Empty;
 
 [JsonPropertyName("email")]
 public string Email { get; set; } = string.Empty;
 
 [JsonPropertyName("role")]
 public string RolName { get; set; } = string.Empty;
 
 [JsonPropertyName("isActive")]
 public bool IsActive { get; set; }
 
 [JsonPropertyName("createdAt")]
 public DateTime? CreatedAt { get; set; }
 }
}
