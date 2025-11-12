using System.Collections.Generic;
using System.Threading.Tasks;
using BirdEntity = BackBird.Api.src.Bird.Modules.Birds.Domain.Entities.Bird;
using BackBird.Api.src.Bird.Modules.Birds.Domain.Repositories;

namespace BackBird.Api.src.Bird.Modules.Birds.Aplication.Queries.GetAllBirds
{
    public class GetAllBirdsHandler
    {
        private readonly IBirdRepository _birdRepository;

        public GetAllBirdsHandler(IBirdRepository birdRepository)
        {
            _birdRepository = birdRepository;
        }

        /// <summary>
        /// Ejecuta la query para obtener todas las aves
        /// </summary>
        public async Task<IEnumerable<BirdEntity>> Handle(GetAllBirdsQuery query)
        {
            return await _birdRepository.GetAllAsync();
        }
    }
}
