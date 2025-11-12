using BackBird.Api.src.Bird.Modules.Birds.Domain.Enums;

namespace BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.CreateBird
{
    public class CreateBirdCommand
    {
        public string CommonName { get; set; }
        public string ScientificName { get; set; }
        public BirdFamily Family { get; set; }
        public ConservationStatus ConservationStatus { get; set; }
        public string Notes { get; set; }
    }
}
