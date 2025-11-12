using System;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace BackBird.Api.src.Bird.Modules.Birds.Domain.Enums
{
    public enum BirdFamily
    {
        [EnumMember(Value = "Rapaces diurnas (halcones, águilas, gavilanes)")]
        [Description("Rapaces diurnas (halcones, águilas, gavilanes)")]
        Accipitridae,
        
        [EnumMember(Value = "Patos, gansos y cisnes")]
        [Description("Patos, gansos y cisnes")]
        Anatidae,
        
        [EnumMember(Value = "Palomas y tórtolas")]
        [Description("Palomas y tórtolas")]
        Columbidae,
        
        [EnumMember(Value = "Colibríes")]
        [Description("Colibríes")]
        Trochilidae,
        
        [EnumMember(Value = "Loros y guacamayas")]
        [Description("Loros y guacamayas")]
        Psittacidae,
        
        [EnumMember(Value = "Búhos y lechuzas")]
        [Description("Búhos y lechuzas")]
        Strigidae,
        
        [EnumMember(Value = "Atrapamoscas y mosqueros")]
        [Description("Atrapamoscas y mosqueros")]
        Tyrannidae,
        
        [EnumMember(Value = "Tangaras")]
        [Description("Tangaras")]
        Thraupidae,
        
        [EnumMember(Value = "Mirlos y zorzales")]
        [Description("Mirlos y zorzales")]
        Turdidae,
        
        [EnumMember(Value = "Pinzones y semilleros")]
        [Description("Pinzones y semilleros")]
        Emberizidae,
        
        [EnumMember(Value = "Pájaros carpinteros")]
        [Description("Pájaros carpinteros")]
        Picidae,
        
        [EnumMember(Value = "Garzas y garcetas")]
        [Description("Garzas y garcetas")]
        Ardeidae,
        
        [EnumMember(Value = "Halcones y cernícalos")]
        [Description("Halcones y cernícalos")]
        Falconidae
    }
}
