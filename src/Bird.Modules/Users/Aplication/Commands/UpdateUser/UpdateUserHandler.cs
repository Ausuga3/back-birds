using BackBird.Api.src.Bird.Modules.Users.Domain.Entities;
using BackBird.Api.src.Bird.Modules.Users.Domain.Enums;
using BackBird.Api.src.Bird.Modules.Users.Domain.Repositories;

namespace BackBird.Api.src.Bird.Modules.Users.Aplication.Commands.UpdateUser
{
    public class UpdateUserHandler
    {
        private readonly IUserRepository _userRepository;

        public UpdateUserHandler(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        /// <summary>
        /// Actualiza un usuario existente con validación de permisos
        /// </summary>
        /// <param name="userId">ID del usuario a actualizar</param>
        /// <param name="command">Datos a actualizar</param>
        /// <param name="actorId">ID del usuario que hace la actualización</param>
        /// <param name="actorRole">Rol del usuario que hace la actualización</param>
        /// <returns>El usuario actualizado</returns>
        /// <exception cref="UnauthorizedAccessException">Si el usuario no tiene permisos</exception>
        /// <exception cref="KeyNotFoundException">Si el usuario no existe</exception>
        public async Task<User> Handle(Guid userId, UpdateUserCommand command, Guid actorId, Role actorRole)
        {
            // Solo admins pueden cambiar roles o estado activo
            // Los usuarios solo pueden actualizar su propio nombre/email
            bool isAdmin = actorRole == Role.Admin;
            bool isSelf = userId == actorId;

            if (!isAdmin && !isSelf)
            {
                throw new UnauthorizedAccessException("No tienes permisos para actualizar este usuario");
            }

            // Si no es admin, no puede cambiar rol ni isActive
            if (!isAdmin && (command.Role.HasValue || command.IsActive.HasValue))
            {
                throw new UnauthorizedAccessException("Solo los administradores pueden cambiar el rol o estado de usuarios");
            }

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
            {
                throw new KeyNotFoundException($"Usuario con ID {userId} no encontrado");
            }

            // Actualizar usando el método público de la entidad
            user.Update(
                command.Name,
                command.Email,
                command.Role,
                command.IsActive
            );

            await _userRepository.UpdateAsync(user);
            return user;
        }
    }
}
