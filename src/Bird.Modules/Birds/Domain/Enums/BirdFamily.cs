using System;
using System.ComponentModel;

namespace Bird.Modules.Birds.Domain.Enums
{
    public enum BirdFamily
    {
        [Description("Rapaces diurnas (halcones, águilas, gavilanes)")]
        Accipitridae,
        [Description("Patos, gansos y cisnes")]
        Anatidae,
        [Description("Palomas y tórtolas")]
        Columbidae,
        [Description("Colibríes")]
        Trochilidae,
        [Description("Loros y guacamayas")]
        Psittacidae,
        [Description("Búhos y lechuzas")]
        Strigidae,
        [Description("Atrapamoscas y mosqueros")]
        Tyrannidae,
        [Description("Tangaras")]
        Thraupidae,
        [Description("Mirlos y zorzales")]
        Turdidae,
        [Description("Pinzones y semilleros")]
        Emberizidae,
        [Description("Pájaros carpinteros")]
        Picidae,
        [Description("Garzas y garcetas")]
        Ardeidae,
        [Description("Halcones y cernícalos")]
        Falconidae
    }
}
