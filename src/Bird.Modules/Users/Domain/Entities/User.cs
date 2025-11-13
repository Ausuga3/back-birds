using BackBird.Api.src.Bird.Modules.Users.Domain.Enums;
using Microsoft.AspNetCore.Identity;

namespace BackBird.Api.src.Bird.Modules.Users.Domain.Entities
{
    public class User
    {
        public Guid Id { get; private set; }
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PasswordHash { get; private set; }
        public Role Role { get; private set; }
        public DateTime Created_At { get; private set; }
        public DateTime Updated_At { get; private set; }
        
        public User(
            string email,
            string passwordHash,
            string name,
            Role role)
        {
            Id = Guid.NewGuid();
            Email = email;
            PasswordHash = passwordHash;
            Name = name;
            Role = role;
            Created_At = DateTime.UtcNow;
            Updated_At = DateTime.UtcNow;
        }

        protected User() { }

        /// <summary>
        /// Actualiza los datos del usuario
        /// </summary>
        public void Update(string? name, string? email, Role? role, bool? isActive)
        {
            if (!string.IsNullOrWhiteSpace(name))
                Name = name;
            
            if (!string.IsNullOrWhiteSpace(email))
                Email = email;
            
            if (role.HasValue)
                Role = role.Value;

            // Nota: isActive no está implementado en la entidad actual
            // Si quieres agregarlo, necesitas agregar la propiedad IsActive
            
            Updated_At = DateTime.UtcNow;
        }
        
    }
}
