using System.ComponentModel;
using System.Runtime.Serialization;

namespace BackBird.Api.src.Bird.Modules.Birds.Domain.Enums
{
    public enum ConservationStatus
    {
        [EnumMember(Value = "Extinta")]
        [Description("Extinta")]
        Extinct,
        
        [EnumMember(Value = "En Peligro")]
        [Description("En Peligro")]
        Endangered,
        
        [EnumMember(Value = "Vulnerable")]
        [Description("Vulnerable")]
        Vulnerable,
        
        [EnumMember(Value = "Casi Amenazada")]
        [Description("Casi Amenazada")]
        NearThreatened,
        
        [EnumMember(Value = "Preocupacion Menor")]
        [Description("Preocupacion Menor")]
        LeastConcern,
        
        [EnumMember(Value = "No Evaluada")]
        [Description("No Evaluada")]
        NotEvaluated
    }
}
