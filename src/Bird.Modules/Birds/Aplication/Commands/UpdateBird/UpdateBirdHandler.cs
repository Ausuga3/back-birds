using System;
using System.Threading.Tasks;
using BirdEntity = BackBird.Api.src.Bird.Modules.Birds.Domain.Entities.Bird;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;

namespace BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.UpdateBird
{
    public class UpdateBirdHandler
    {
        private readonly IBirdRepository _birdRepository;

        public UpdateBirdHandler(IBirdRepository birdRepository)
        {
            _birdRepository = birdRepository;
        }

        /// <summary>
        /// Actualiza un ave existente con validación de permisos
        /// </summary>
        /// <param name="command">Datos actualizados del ave</param>
        /// <param name="userId">ID del usuario que intenta editar</param>
        /// <param name="isAdmin">Si el usuario es administrador</param>
        /// <returns>El ave actualizada</returns>
        /// <exception cref="UnauthorizedAccessException">Si el usuario no tiene permisos</exception>
        /// <exception cref="KeyNotFoundException">Si el ave no existe</exception>
        public async Task<BirdEntity> Handle(UpdateBirdCommand command, string userId, bool isAdmin)
        {
            // 1. Obtener el ave existente
            var bird = await _birdRepository.GetByIdAsync(command.Id);
            
            if (bird == null)
            {
                throw new KeyNotFoundException($"Ave con ID {command.Id} no encontrada");
            }

            // 2. Validar permisos: solo admin o el creador pueden editar
            if (!bird.CanBeEditedBy(userId, isAdmin))
            {
                throw new UnauthorizedAccessException(
                    "No tienes permisos para editar esta ave. Solo el creador o un administrador pueden editarla.");
            }

            // 3. Actualizar los datos del ave
            bird.Update(
                commonName: command.CommonName,
                scientificName: command.ScientificName,
                family: command.Family,
                conservationStatus: command.ConservationStatus,
                notes: command.Notes
            );

            // 4. Persistir los cambios
            await _birdRepository.UpdateAsync(bird);

            // 5. Retornar el ave actualizada
            return bird;
        }
    }
}
