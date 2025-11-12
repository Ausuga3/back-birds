using System;
using System.Threading.Tasks;
using BirdEntity = BackBird.Api.src.Bird.Modules.Birds.Domain.Entities.Bird;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;

namespace BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.CreateBird
{
    public class CreateBirdHandler
    {
        private readonly IBirdRepository _birdRepository;

        public CreateBirdHandler(IBirdRepository birdRepository)
        {
            _birdRepository = birdRepository;
        }

        /// <summary>
        /// Ejecuta el comando de crear un ave.
        /// </summary>
        /// <param name="command">Datos del ave a crear</param>
        /// <param name="createdBy">Identificador del usuario que crea el ave (viene del claim/token)</param>
        /// <returns>Respuesta con el ave completa creada</returns>
        public async Task<BirdEntity> Handle(CreateBirdCommand command, string createdBy)
        {
            // 1. Crear la entidad de dominio usando el constructor
            var bird = new BirdEntity(
                name: command.CommonName,
                scientificName: command.ScientificName,
                family: command.Family,
                conservationStatus: command.ConservationStatus,
                notes: command.Notes,
                createdBy: createdBy
            );

            // 2. Persistir usando el repositorio (capa de infraestructura)
            await _birdRepository.AddAsync(bird);

            // 3. Retornar el ave completa
            return bird;
        }
    }
}
