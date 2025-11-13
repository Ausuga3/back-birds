using BackBird.Api.src.Bird.Modules.Users.Domain.Enums;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.UpdateUser
{
    public class UpdateUserCommand
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public Role? Role { get; set; }
        public bool? IsActive { get; set; }
    }
}
