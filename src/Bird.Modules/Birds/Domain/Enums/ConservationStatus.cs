using System.ComponentModel;

namespace Bird.Modules.Birds.Domain.Enums
{
    public enum ConservationStatus
    {
        [Description("Extinta")]
        Extinct,
        [Description("En Peligro")]
        Endangered,
        [Description("Vulnerable")]
        Vulnerable,
        [Description("Casi Amenazada")]
        NearThreatened,
        [Description("Preocupacion Menor")]
        LeastConcern,
        [Description("No Evaluada")]
        NotEvaluated
    }
}
