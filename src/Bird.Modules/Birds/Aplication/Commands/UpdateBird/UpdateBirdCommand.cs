using BackBird.Api.src.Bird.Modules.Birds.Domain.Enums;

namespace BackBird.Api.src.Bird.Modules.Birds.Aplication.Commands.UpdateBird
{
    public class UpdateBirdCommand
    {
        public Guid Id { get; set; }
        public string CommonName { get; set; } = string.Empty;
        public string ScientificName { get; set; } = string.Empty;
        public BirdFamily Family { get; set; }
        public ConservationStatus ConservationStatus { get; set; }
        public string Notes { get; set; } = string.Empty;
    }
}
