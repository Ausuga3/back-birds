using BackBird.Api.src.Bird.Modules.Birds.Domain.Enums;
namespace BackBird.Api.src.Bird.Modules.Birds.Domain.Entities
{
  public class Bird
  {
    public Guid Id { get; private set; }
    public string CommonName { get; private set; } = string.Empty;
    public string ScientificName { get; private set; } = string.Empty;
    public BirdFamily Family { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public ConservationStatus ConservationStatus { get; private set; }
    public DateTime Created_At { get; private set; }
    public DateTime Updated_At { get; private set; }
    public string Created_By { get; private set; } = string.Empty;

    // Constructor sin parámetros para EF Core
    private Bird() { }

    // Constructor para crear instancias desde el dominio
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

    /// <summary>
    /// Actualiza los datos del ave
    /// </summary>
    public void Update(
      string commonName,
      string scientificName,
      BirdFamily family,
      ConservationStatus conservationStatus,
      string notes)
    {
      CommonName = commonName;
      ScientificName = scientificName;
      Family = family;
      ConservationStatus = conservationStatus;
      Notes = notes;
      Updated_At = DateTime.UtcNow;
    }

    /// <summary>
    /// Verifica si un usuario puede editar esta ave
    /// </summary>
    public bool CanBeEditedBy(string userId, bool isAdmin)
    {
      return isAdmin || Created_By == userId;
    }
  }
}
