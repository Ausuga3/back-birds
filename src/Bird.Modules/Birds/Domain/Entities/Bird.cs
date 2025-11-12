using System.Net.Sockets;
using Bird.Modules.Birds.Domain.Enums;
namespace BackBird.Api.src.Bird.Modules.Birds.Domain.Entities
{
  public class Bird
  {
    public Guid Id { get; private set; }
    public string CommonName { get; private set; }
    public string ScientificName { get; private set; }
    public BirdFamily Family { get; private set; }
    public string Notes { get; private set; }
    public ConservationStatus ConservationStatus { get; private set; }
    public DateTime Created_At { get; private set; }
    public DateTime Updated_At { get; private set; }
    public string Created_By { get; private set; }

    public Bird(
      string name,
      string scientificName,
      BirdFamily family,
      ConservationStatus conservationStatus,
      string notes,
      string createdBy = "system"
    )
    {
      Id = Guid.NewGuid();
      CommonName = name;
      ScientificName = scientificName;
      Family = family;
      ConservationStatus = conservationStatus;
      Notes = notes;
      Created_At = DateTime.UtcNow;
      Updated_At = DateTime.UtcNow;
      Created_By = createdBy;
    }
  }
}
