using System;
using System.ComponentModel;
using System.Reflection;

namespace BackBird.Api.src.Bird.Modules.Birds.Domain.Enums
{
    public static class EnumExtensions
    {
        public static string GetDescription(this Enum value)
        {
            if (value == null) return string.Empty;

            var fi = value.GetType().GetField(value.ToString());
            if (fi == null) return value.ToString();

            var attr = fi.GetCustomAttribute<DescriptionAttribute>();
            return attr?.Description ?? value.ToString();
        }
    }
}
